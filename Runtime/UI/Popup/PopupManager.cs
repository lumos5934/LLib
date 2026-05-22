using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;


namespace LLib
{
    public class PopupManager : MonoBehaviour, IPreInitializable
    {
        [Header("Sorting")]
        [SerializeField] private string _layer;
        [SerializeField] private int _minOrder = 1000;
        [SerializeField, Min(2)] private int _orderSpacing = 2;
        
        [Space(15f)]
        [SerializeField] private Canvas _dimmerCanvas;

        private Dictionary<Type, UIPopup> _prefabsByType = new();
        private Dictionary<Type, UIPopup> _instancesByType = new();
        private List<UIPopup> _openedPopups = new();
        private Camera _camera;
        
        
        private void Awake()
        {
            Services.Register(this);
        }
        
        
        public UniTask<bool> InitAsync(PreInitContext ctx)
        {
            _camera = GetComponentInChildren<Camera>();
            if (_camera == null)
                return UniTask.FromResult(false);

            
            _camera.clearFlags = CameraClearFlags.Depth;

            if (_dimmerCanvas != null)
            {
                _dimmerCanvas.sortingLayerName = _layer;
                _dimmerCanvas.worldCamera = _camera;
                _dimmerCanvas.gameObject.SetActive(false);
            }
            
            UpdateCameraStack();
            
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                UpdateCameraStack();
            };

            return UniTask.FromResult(true);
        }

        
        public void RegisterPrefab(UIPopup prefab)
        {
            if (prefab == null) 
                return;

            Type type = prefab.GetType(); 
            _prefabsByType[type] = prefab;
        }
        
      
        public void UnregisterPrefab<T>() where T : UIPopup
        {
            UnregisterPrefab(typeof(T));
        }
        
        
        public void UnregisterPrefab(UIPopup prefab)
        {
            if (prefab == null) 
                return;
            
            Type type = prefab.GetType(); 
            UnregisterPrefab(type);
        }
        
        
        private void UnregisterPrefab(Type type)
        {
            _prefabsByType.Remove(type);
            
            if (_instancesByType.TryGetValue(type, out var instance))
            {
                if (instance != null)
                {
                    _openedPopups.Remove(instance); 
                    Destroy(instance.gameObject);
                }
                
                _instancesByType.Remove(type);
                UpdateOrders();
            }
        }

        
        private T Get<T>() where T : UIPopup
        {
            if (_instancesByType.TryGetValue(typeof(T), out var containsPopup))
            {
                return containsPopup as T;
            }
            
            return CreateInstance<T>();
        }

        
        private T CreateInstance<T>() where T : UIPopup
        {
            _prefabsByType.TryGetValue(typeof(T), out UIPopup prefab);
            if (prefab == null)
                return null;

            var newPopup = Instantiate(prefab, transform);
            newPopup.Init();
            newPopup.Canvas.worldCamera = _camera;
            newPopup.Canvas.sortingLayerName = _layer;
            newPopup.gameObject.SetActive(false);

            _instancesByType[typeof(T)] = newPopup; 

            return newPopup as T;
        }

        
        public T Open<T>() where T : UIPopup
        {
            var popup = Get<T>();
            if (popup == null)
                return null;
            
            OnOpen(popup);
            popup.Open();
            
            return popup;
        }

        
        private void OnOpen(UIPopup popup)
        {
            if (_openedPopups.Contains(popup))
            {
                if (_openedPopups[^1] == popup)
                {
                    UpdateOrders();
                    return;
                }

                _openedPopups.Remove(popup);
            }
            
            _openedPopups.Add(popup);
            UpdateOrders();
        }

        
        public void Close()
        {
            if (_openedPopups.Count == 0)
                return;

            int lastIndex = _openedPopups.Count - 1;
            var popup = _openedPopups[lastIndex];

            _openedPopups.RemoveAt(lastIndex);
            popup.Close();

            UpdateOrders();
        }

        
        public void Close<T>() where T : UIPopup
        {
            for (int i = _openedPopups.Count - 1; i >= 0; i--)
            {
                if (_openedPopups[i] is T popup)
                {
                    _openedPopups.RemoveAt(i);
                    popup.Close();
                    
                    UpdateOrders();
                    return;
                }
            }
        }

        
        public void CloseAll()
        {
            for (int i = _openedPopups.Count - 1; i >= 0; i--)
            {
                _openedPopups[i].Close();
            }
            
            _openedPopups.Clear();
            
            if (_dimmerCanvas != null)
            {
                _dimmerCanvas.gameObject.SetActive(false);
            }
        }
        
        
        private void UpdateOrders()
        {
            int dimmerOrder = -1;
            
            for (int i = 0; i < _openedPopups.Count; i++)
            {
                int order = _minOrder + (i + 1) * _orderSpacing;
                var target = _openedPopups[i];

                target.Canvas.sortingOrder = order;
                
                if (target.IsModal)
                {
                    dimmerOrder = order - 1;
                }
            }
            
            if (_dimmerCanvas != null)
            {
                if (dimmerOrder > -1)
                {
                    _dimmerCanvas.gameObject.SetActive(true);
                    _dimmerCanvas.sortingOrder = dimmerOrder;
                }
                else
                {
                    _dimmerCanvas.gameObject.SetActive(false);
                }
            }
        }
        
        
        private void UpdateCameraStack()
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                var mainCamData = mainCam.GetUniversalAdditionalCameraData();
                var popupCamData = _camera.GetUniversalAdditionalCameraData();
                
                if (!mainCamData.cameraStack.Contains(_camera))
                {
                    mainCamData.cameraStack.Add(_camera);
                    
                    _camera.targetTexture = mainCam.targetTexture;
                    _camera.targetDisplay = mainCam.targetDisplay;
                    _camera.allowMSAA = mainCam.allowMSAA;
                
                    popupCamData.antialiasing = mainCamData.antialiasing;
                    popupCamData.antialiasingQuality = mainCamData.antialiasingQuality;
                }
            }
        }
    }
}