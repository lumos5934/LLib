using System;
using UnityEditor;
using UnityEngine;

namespace LLib.Editor
{
    public class EditorGameObjectMenu
    {
        [MenuItem("GameObject/[ LLib ]/UI/World Button", false, 0)]
        private static void CreateWorldButton(MenuCommand menuCommand)
        {
            CreateNewObject(menuCommand, "WorldButton", new []
            {
                typeof(SpriteRenderer),
                typeof(WorldButton),
            });
        }

        private static void CreateNewObject(MenuCommand menuCommand, string name, Type[] addComponents)
        {
            GameObject parent = menuCommand.context as GameObject;
            GameObject createObject = new GameObject(name);
            
            if (parent != null)
            {
                GameObjectUtility.SetParentAndAlign(createObject, parent);
            }

            for (int i = 0; i < addComponents.Length; i++)
            {
                createObject.AddComponent(addComponents[i]);
            }
            
            Selection.activeObject = createObject;
        }
        
        private static void CreateResource(MenuCommand menuCommand, string path)
        {
            var resource = Resources.Load<GameObject>(path);
            if (resource == null) return;
            
            GameObject parent = menuCommand.context as GameObject;
            GameObject createObject = (GameObject)PrefabUtility.InstantiatePrefab(resource);

            if (parent != null)
            {
                GameObjectUtility.SetParentAndAlign(createObject, parent);
            }

            Undo.RegisterCreatedObjectUndo(createObject, "Create " + createObject.name);
            
            Selection.activeObject = createObject;
        }
    }
}