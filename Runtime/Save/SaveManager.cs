using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace LLib
{
    public class SaveManager : SingletonGlobal<SaveManager>, ISaveManager
    {
        [SerializeField] private BaseDataSource _saveDataSource;

        private readonly Dictionary<Type, object> _saveDataDict = new();



        public async UniTask SaveAsync<T>(T data)
        {
            if (_saveDataSource == null)
            {
                Debug.LogWarning("SAVE : data source not found");
                return;
            }

            var type = typeof(T);

            _saveDataDict[type] = data;

            try
            {
                await _saveDataSource.WriteAsync(data);
                Debug.Log($"SAVE SUCCESS ({type.Name})");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        public async UniTask<T> LoadAsync<T>()
        {
            if (_saveDataSource == null)
            {
                Debug.LogWarning("data source not found");
                return default;
            }

            if (_saveDataDict.TryGetValue(typeof(T), out var cached))
                return (T)cached;

            var type = typeof(T);

            try
            {
                var result = await _saveDataSource.ReadAsync<T>();

                if (!EqualityComparer<T>.Default.Equals(result, default))
                {
                    _saveDataDict[type] = result;
                    Debug.Log($"LOAD SUCCESS ({type.Name})");
                }
                else
                {
                    Debug.LogWarning($"LOAD : save data not found ({type.Name})");
                }

                return result;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
    }
}