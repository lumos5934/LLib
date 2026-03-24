using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LumosLib
{
    public static class Services
    {
        private static Dictionary<Type, object> _services = new();
        
        
        public static void Register<T>(T service) where T : class
        {
            bool contains = _services.ContainsKey(typeof(T));
            if (contains)
            {
                if (service is MonoBehaviour containsMono)
                {
                    Object.Destroy(containsMono.gameObject);
                }
                
                DebugUtil.LogWarning($"{typeof(T)} : ALREADY REGISTERED. Ignore new one.", " Fail Register ");
                return;
            }
            
            if (service is MonoBehaviour mono)
            {
                Object.DontDestroyOnLoad(mono.gameObject);
            }
            
            _services[typeof(T)] = service;
        }

        
        public static void Unregister<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                if (service is MonoBehaviour mono && mono != null)
                {
                    Object.Destroy(mono.gameObject);
                }

                _services.Remove(typeof(T));
            }
        }
        
        
        public static T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }

            DebugUtil.LogWarning($"{typeof(T)}", " NOT REGISTERED ");
            return null;
        }
    }
}