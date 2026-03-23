using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace LumosLib
{
    public class PreInitContext
    {
        private readonly Dictionary<Type, IPreInitializable> _instances = new();
        private readonly Dictionary<Type, UniTaskCompletionSource<bool>> _completionSources = new();

        
        private UniTaskCompletionSource<bool> GetSource(Type type)
        {
            if (!_completionSources.TryGetValue(type, out var completionSource))
            {
                completionSource = new UniTaskCompletionSource<bool>();
                _completionSources[type] = completionSource;
            }
            
            return completionSource;
        }

        
        public void Register(IPreInitializable instance)
        {
            _instances[instance.GetType()] = instance;
        }
        
        public void SetTaskResult(Type type, bool success)
        {
            // 작업 결과 연결
            GetSource(type).TrySetResult(success);
        }
        

        public async UniTask<T> GetAsync<T>() where T : class, IPreInitializable
        {
            return await WaitAndGetAsync(typeof(T)) as T;
        }

        
        public async UniTask<T> GetAsync<T>(T instance) where T : class, IPreInitializable
        {
            if (instance == null) 
                return null;
            
            return await WaitAndGetAsync(instance.GetType()) as T;
        }

        
        private async UniTask<IPreInitializable> WaitAndGetAsync(Type type)
        {
            if (!_instances.TryGetValue(type, out var instance))
            {
                return null;
            }

            bool success = await GetSource(type).Task;
            return success ? instance : null;
        }

        public void Clear()
        {
            _instances.Clear();
            _completionSources.Clear();
        }
    }
}