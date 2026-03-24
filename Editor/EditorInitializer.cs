using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace LumosLib.Editor
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
            var settings = LumosLibSettings.Instance;
            if (settings != null)
                return;

            string assetPath = $"Assets/{nameof(LumosLibSettings)}.asset";
            settings = AssetDatabase.LoadAssetAtPath<LumosLibSettings>(assetPath);

            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<LumosLibSettings>();
                AssetDatabase.CreateAsset(settings, assetPath);
                AssetDatabase.SaveAssets();
            }

            LumosLibSettings.Instance = settings;
            AssetDatabase.Refresh();
        }
    }
}