using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace LumosLib
{
    public static class PreInitializer
    {
        private static int _curCount;
        private static int _maxCount;

        private static bool _isInitialized;

        private static UniTaskCompletionSource _initBarrier;
        
        public static bool IsInitialized => _isInitialized;
        public static float InitProgress =>
            _maxCount == 0 ? 1f : (float)_curCount / _maxCount;

        

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Boot()
        {
            _initBarrier = new UniTaskCompletionSource();
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            var libSettings = Resources.Load<LumosLibSettings>(nameof(LumosLibSettings));
            
            if (libSettings == null || !libSettings.UsePreInitialize)
            {
                _initBarrier.TrySetResult();
                return;
            }

            Initialize(libSettings).Forget();
        }
        
        public static UniTask WaitInitAsync()
        {
            if (_isInitialized)
                return UniTask.CompletedTask;

            return _initBarrier.Task;
        }

        private static async UniTask Initialize(LumosLibSettings libSettings)
        {
            DebugUtil.Log("", "------ INITIALIZE START");

            var totalSW = System.Diagnostics.Stopwatch.StartNew();
            var context = new PreInitContext();
            var preloadSW = System.Diagnostics.Stopwatch.StartNew();
            
            foreach (var prefab in libSettings.PreloadObjects)
            {
                if (prefab == null)
                    continue;

                var obj = Object.Instantiate(prefab);
                obj.name = prefab.name;
            }
            
            DebugUtil.Log("", $"PRELOAD FINISH ({preloadSW.ElapsedMilliseconds:F2} ms)");

            
            var allInitializable = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<IPreInitializable>()
                .ToList();
            
            _maxCount = allInitializable.Count;
            
            var taskList = new List<UniTask<bool>>();
            
            foreach (var initializable in allInitializable)
            {
                _curCount++;
                
                var task = InitializeTarget(context, initializable);
                taskList.Add(task);
                
                context.Register(initializable, task);
            }
            
            await UniTask.WhenAll(taskList);
            
            DebugUtil.Log("", $"------ INITIALIZE FINISH ({totalSW.ElapsedMilliseconds:F2} ms)");

            FinishInit();
        }
        
        
        private static void FinishInit()
        {
            _isInitialized = true;
            _initBarrier.TrySetResult();
        }
        
        
        private static async UniTask<bool> InitializeTarget(PreInitContext ctx, IPreInitializable target)
        {
            string targetName = target.GetType().Name;
            float startTime = Time.realtimeSinceStartup;

            try
            {
                bool success = await target.InitAsync(ctx);
                float elapsed = (Time.realtimeSinceStartup - startTime) * 1000f;

                if (success)
                {
                    DebugUtil.Log(targetName, $"INIT SUCCESS ({elapsed:F2} ms) [{_curCount}/{_maxCount}]");
                }
                else
                {
                    DebugUtil.LogError(targetName, $"INIT FAIL");
                }

                return success;
            }
            catch (System.Exception e)
            {
                DebugUtil.LogError(targetName, $"INIT EXCEPTION: {e.Message}");
                return false;
            }
        }
    }
}