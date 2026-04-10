using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace LLib
{
    public static class PreInitializer
    {
        private static int _maxCount;
        private static int _successCount;
        private static int _failCount;
        private static bool _isInitialized;
        private static UniTaskCompletionSource _initBarrier;
        private static Stopwatch _initStopwatch;


        public static bool IsInitialized => _isInitialized;
        public static float InitProgress =>
            _maxCount == 0 ? 1f : (float)(_successCount + _failCount) / _maxCount;
        

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Boot()
        {
            _initStopwatch = Stopwatch.StartNew();
            
            _initBarrier = new UniTaskCompletionSource();
            
            var libSettings = LLibSettings.Instance;
            if (libSettings == null)
                return;
            
            Preload(libSettings);
            DebugUtil.Log("", $"------ PRELOAD FINISH ({_initStopwatch.ElapsedMilliseconds:F2} ms)");
        }
        
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            var libSettings = LLibSettings.Instance;
            if (libSettings == null || !libSettings.UsePreInitialize)
            {
                _initBarrier.TrySetResult();
                return;
            }

            Initialize().Forget();
        }
        
        
        public static UniTask WaitInitAsync()
        {
            if (_isInitialized)
                return UniTask.CompletedTask;

            return _initBarrier.Task;
        }

        
        private static async UniTask Initialize()
        {
            var allInitializable = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<IPreInitializable>()
                .ToList();
            
            _maxCount = allInitializable.Count;
            
            _initStopwatch = Stopwatch.StartNew();
            
            DebugUtil.Log("", $"------ INITIALIZE START (TOTAL : {_maxCount})");
            
            
            var context = new PreInitContext();
            
            foreach (var initializable in allInitializable)
            {
                context.Register(initializable);
            }
            
            
            var taskList = new List<UniTask<bool>>();
            
            foreach (var initializable in allInitializable)
            {
                var task = InitializeTarget(context, initializable, _initStopwatch);
                taskList.Add(task);
            }
            
            
            await UniTask.WhenAll(taskList);
            
            FinishInit();
            context.Clear();
            
            DebugUtil.Log("", $"------ INITIALIZE FINISH (SUCCESS : {_successCount} , FAIL : {_failCount})");
        }
        
        
        private static void FinishInit()
        {
            _isInitialized = true;
            _initBarrier.TrySetResult();
        }
        
        
        private static async UniTask<bool> InitializeTarget(PreInitContext ctx, IPreInitializable target, System.Diagnostics.Stopwatch initSW)
        {
            
            string targetName = target.GetType().Name;

            try
            {
                bool success = await target.InitAsync(ctx);
                
                ctx.SetTaskResult(target.GetType(), success);

                float elapsed = initSW.ElapsedMilliseconds;

                if (success)
                {
                    _successCount++;
                    DebugUtil.Log(targetName, $"INIT SUCCESS ({elapsed:F2} ms)");
                }
                else
                {
                    _failCount++;
                    DebugUtil.LogError(targetName, $"INIT FAIL ({elapsed:F2} ms)");
                }
                
                return success;
            }
            catch (System.Exception e)
            {
                _failCount++;
                
                ctx.SetTaskResult(target.GetType(), false);
                
                DebugUtil.LogError(targetName, $"INIT EXCEPTION: {e.Message}");
                
                return false;
            }
        }

        
        private static void Preload(LLibSettings settings)
        {
            foreach (var prefab in settings.PreloadObjects)
            {
                if (prefab == null)
                    continue;

                var obj = Object.Instantiate(prefab);
                obj.name = prefab.name;
            }
        }
    }
}