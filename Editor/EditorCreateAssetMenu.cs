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
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
                path = "Assets";
            else if (!Directory.Exists(path))
                path = Path.GetDirectoryName(path);

            return path;
        }

        public static GameObject CreatePrefab<T>(UnityAction<GameObject> onObjCreated = null) where T : Component
        {
            var typeName = typeof(T).Name;

            var obj = new GameObject(typeName);
            obj.AddComponent<T>();

            onObjCreated?.Invoke(obj);

            var prefabPath = Path.Combine(GetCurrentPath(), obj.name + ".prefab");

            var prefab = PrefabUtility.SaveAsPrefabAsset(obj, prefabPath);

            Selection.activeObject = null;
            Object.DestroyImmediate(obj);
            AssetDatabase.Refresh();

            return prefab;
        }

        public static void CreateScript(string scriptName, string template)
        {
            var path = GetCurrentPath();


            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<OnCreateScript>(),
                Path.Combine(path, $"{scriptName}.cs"),
                null,
                template
            );
        }

        /// <summary>
        ///     Prefabs
        /// </summary>
        [MenuItem("Assets/Create/[ LLib ]/Prefabs/Manager/Camera", false, int.MinValue)]
        public static void CreateCameraManagerPrefab()
        {
            CreatePrefab<CameraManager>();
        }


        [MenuItem("Assets/Create/[ LLib ]/Prefabs/Manager/Resource", false, int.MinValue)]
        public static void CreateResourceManagerPrefab()
        {
            CreatePrefab<ResourceManager>();
        }

        [MenuItem("Assets/Create/[ LLib ]/Prefabs/Manager/Popup", false, int.MinValue)]
        public static void CreatePopupManagerPrefab()
        {
            CreatePrefab<PopupManager>(obj =>
            {
                var cameraObj = new GameObject("Camera");
                cameraObj.transform.SetParent(obj.transform);
                var camera = cameraObj.AddComponent<Camera>();
                var data = camera.GetUniversalAdditionalCameraData();
                data.renderType = CameraRenderType.Overlay;
            });
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

        internal class OnCreateScript : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var className = Path.GetFileNameWithoutExtension(pathName);
                var finalContent = resourceFile.Replace("#SCRIPTNAME#", className);

                File.WriteAllText(pathName, finalContent);
                AssetDatabase.ImportAsset(pathName);
                var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(pathName);
                ProjectWindowUtil.ShowCreatedAsset(asset);
            }
        }
    }
}