using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LLib
{
    [Serializable]
    public class ResourceGroup
    {
        [Group("Folder Path")] [SerializeField] [HideLabel] [Required]
        private string _folderPath;

        [Group("Label ")] [SerializeField] [HideLabel] [Required]
        private string _label;


        public string FolderPath => _folderPath;
        public string Label => _label;
        public Dictionary<string, Object> Resources { get; } = new();


        public void SetResources(Object[] resources)
        {
            foreach (var resource in resources) Resources.TryAdd(resource.name, resource);
        }
    }
}