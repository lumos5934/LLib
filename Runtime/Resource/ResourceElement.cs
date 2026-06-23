using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LLib
{
    [Serializable]
    public class ResourceElement
    {
        [HideLabel] public string key;
        [HideInInspector] public List<Object> resources;

        public T GetResource<T>()
        {
            foreach (var resource in resources)
                if (resource is T t)
                    return t;

            return default;
        }
    }
}