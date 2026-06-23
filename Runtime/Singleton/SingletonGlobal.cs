using UnityEngine;

namespace LLib
{
    public class SingletonGlobal<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool applicationIsQuitting;

        private static T _instance;

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting) return null;

                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();

                    if (_instance == null)
                    {
                        var go = new GameObject(typeof(T).Name);
                        _instance = go.AddComponent<T>();
                    }

                    DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }


        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this) 
                _instance = null;
        }

        protected virtual void OnApplicationQuit()
        {
            applicationIsQuitting = true;
        }
    }
}