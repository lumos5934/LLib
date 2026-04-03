using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace LLib.Editor
{
    public static class EditorInitializer
    {
        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            EditorApplication.delayCall += OnEditorFullyLoaded;
        }
        
        private static void OnEditorFullyLoaded()
        {
            string assetPath = $"Assets/{nameof(LLibSettings)}.asset";
            
            
            var settings = AssetDatabase.LoadAssetAtPath<LLibSettings>(assetPath);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<LLibSettings>();
                AssetDatabase.CreateAsset(settings, assetPath);
                AssetDatabase.SaveAssets();
            }

            
            var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
            preloadedAssets.RemoveAll(a => a == null);
            
            if (!preloadedAssets.Contains(settings))
            {
                preloadedAssets.Add(settings);
                PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
            }
        }
    }
}