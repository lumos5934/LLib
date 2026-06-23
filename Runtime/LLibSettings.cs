using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;

namespace LLib
{
    [CreateAssetMenu(fileName = "LLibSettings", menuName = "[ LLib ]/Scriptable Objects/Settings",
        order = int.MinValue)]
    public class LLibSettings : ScriptableObject
    {
        private static LLibSettings _instance;

        [Title("Preload")] [SerializeField] private bool _usePreInitialize;

        [PropertySpace(10f)] [SerializeField] private List<GameObject> _preloadObjects;

        public static LLibSettings Instance
        {
            get
            {
                if (_instance == null) _instance = Resources.FindObjectsOfTypeAll<LLibSettings>().FirstOrDefault();
                return _instance;
            }
        }


        public bool UsePreInitialize => _usePreInitialize;
        public List<GameObject> PreloadObjects => _preloadObjects;
    }
}