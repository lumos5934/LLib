using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TriInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LumosLib
{
    public class ResourceManager : MonoBehaviour, IResourceManager, IPreInitializable
    {
        [SerializeField, 
         TableList(Draggable = true,
             HideAddButton = false,
             HideRemoveButton = false,
             AlwaysExpanded = false)] 
        private List<ResourceGroup> _groups;
        
        private readonly Dictionary<string, Object> _allResources = new();
        private readonly Dictionary<string, List<ResourceGroup>> _allGroups = new();


        
        private void Awake()
        {
            Services.Register<IResourceManager>(this);
        }

        
        public UniTask<bool> InitAsync(PreInitContext ctx)
        {
            _allResources.Clear();
            _allGroups.Clear();
            
            foreach (var group in _groups)
            {
                if (string.IsNullOrEmpty(group.Label) || 
                    string.IsNullOrEmpty(group.FolderPath))
                    continue;

                if (!_allGroups.ContainsKey(group.Label))
                {
                    _allGroups[group.Label] = new List<ResourceGroup>();
                }
                
                _allGroups[group.Label].Add(group);

                var resources = Resources.LoadAll<Object>(group.FolderPath);
                group.SetResources(resources);

                foreach (var resource in resources)
                {
                    if (!_allResources.TryAdd(resource.name, resource))
                    {
                        DebugUtil.LogWarning($"fail add {group.Label} resources: {resource.name} (label : {group.Label}, path: {group.FolderPath})", "Duplicate Name");
                    }
                }
            }

            return UniTask.FromResult(true);
        }

        
        public T Get<T>(string assetName) where T : Object
        {
            if (_allResources.TryGetValue(assetName, out var resource))
            {
                return GetTypeResource<T>(resource);
            }

            return null;
        }

        public T Get<T>(string label, string assetName) where T : Object
        {
            if (string.IsNullOrEmpty(label)) 
                return Get<T>(assetName);

            if (_allGroups.TryGetValue(label, out var groups))
            {
                foreach (var group in groups)
                {
                    if (group.Resources.TryGetValue(assetName, out var resource))
                    {
                        var target = GetTypeResource<T>(resource);
                        if (target != null) return target;
                    }
                }
            }
            return null;
        }

        public List<T> GetAll<T>(string label) where T : Object
        {
            if (string.IsNullOrEmpty(label)) 
                return GetAll<T>();

            var result = new List<T>();
            
            if (_allGroups.TryGetValue(label, out var groups))
            {
                foreach (var group in groups)
                {
                    foreach (var resource in group.Resources.Values)
                    {
                        var target = GetTypeResource<T>(resource);
                        if (target != null)
                        {
                            result.Add(target);
                        }
                    }
                }
            }
            
            return result;
        }

        public List<T> GetAll<T>() where T : Object
        {
            var result = new List<T>();

            foreach (var resource in _allResources.Values)
            {
                var target = GetTypeResource<T>(resource);
                if (target != null)
                {
                    result.Add(target);
                }
            }

            return result;
        }
        
        private T GetTypeResource<T>(Object resource) where T : Object
        {
            switch (resource)
            {
                case T target:
                    return target;
                    
                case GameObject go:
                    var component = go.GetComponent<T>();
                    if (component != null)
                    {
                        return component;
                    }

                    break;
            }

            return default;
        }


      
    }
}