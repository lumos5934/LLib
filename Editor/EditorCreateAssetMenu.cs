using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace LLib.Editor
{
    public static class EditorCreateAssetMenu
    {
        private static string GetCurrentPath()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
                path = "Assets";
            else if (!Directory.Exists(path))
                path = Path.GetDirectoryName(path);

            return path;
        }
        
        public static GameObject CreatePrefab<T>(UnityAction<GameObject> onObjCreated = null) where T : Component
        {
            string typeName = typeof(T).Name;
            
            GameObject obj = new GameObject(typeName);
            obj.AddComponent<T>();
            
            onObjCreated?.Invoke(obj);
            
            string prefabPath = Path.Combine(GetCurrentPath(), obj.name + ".prefab");

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(obj, prefabPath);
            
            Selection.activeObject = null;
            Object.DestroyImmediate(obj);
            AssetDatabase.Refresh();
            
            return prefab;
        }
        
        public static void CreateScript(string scriptName, string template)
        {
            string path = GetCurrentPath();
            
            
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<OnCreateScript>(),
                Path.Combine(path, $"{scriptName}.cs"),
                null,
                template
            );
        }
        
        internal class OnCreateScript : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                string className = Path.GetFileNameWithoutExtension(pathName);
                string finalContent = resourceFile.Replace("#SCRIPTNAME#", className);

                File.WriteAllText(pathName, finalContent);
                AssetDatabase.ImportAsset(pathName);
                var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(pathName);
                ProjectWindowUtil.ShowCreatedAsset(asset);
            }
        }
        
        /// <summary>
        /// Prefabs
        /// </summary>
        
        [MenuItem("Assets/Create/[ LLib ]/Prefabs/Manager/Resource", false, int.MinValue)]
        public static void CreateResourceManagerPrefab()
        {
            CreatePrefab<ResourceManager>();
        }
        
        [MenuItem("Assets/Create/[ LLib ]/Prefabs/Manager/Audio", false, int.MinValue)]
        public static void CreateAudioManagerPrefab()
        {
            CreatePrefab<AudioManager>();
        }
        
        [MenuItem("Assets/Create/[ LLib ]/Prefabs/Manager/Popup", false, int.MinValue)]
        public static void CreatePopupManagerPrefab()
        {
            CreatePrefab<PopupManager>(obj =>
            {
                GameObject cameraObj = new GameObject("Camera");
                cameraObj.transform.SetParent(obj.transform);
                var camera = cameraObj.AddComponent<Camera>();
                var data = camera.GetUniversalAdditionalCameraData();
                data.renderType =  CameraRenderType.Overlay;
            });
        }
        
        [MenuItem("Assets/Create/[ LLib ]/Prefabs/Manager/Pool", false, int.MinValue)]
        public static void CreatePoolManagerPrefab()
        {
            CreatePrefab<PoolManager>();
        }
        
        [MenuItem("Assets/Create/[ LLib ]/Prefabs/Manager/Pointer", false, int.MinValue)]
        public static void CreatePointerManagerPrefab()
        {
            CreatePrefab<PointerManager>();
        }
        
        [MenuItem("Assets/Create/[ LLib ]/Prefabs/Manager/Tutorial", false, int.MinValue)]
        public static void CreateTutorialManagerPrefab()
        {
            CreatePrefab<TutorialManager>();
        }
        
        [MenuItem("Assets/Create/[ LLib ]/Prefabs/Manager/Save", false, int.MinValue)]
        public static void CreateSaveManagerPrefab()
        {
            CreatePrefab<SaveManager>();
        }
        
        [MenuItem("Assets/Create/[ LLib ]/Prefabs/Audio Player", false, int.MinValue)]
        public static void CreateAudioPlayerPrefab()
        {
            CreatePrefab<AudioPlayer>();
        }
    }
}