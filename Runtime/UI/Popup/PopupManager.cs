using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TriInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace LumosLib
{
    public class PopupManager : BasePopupManager
    {
        [InfoBox("Requirement : IResourceManager")]
        [SerializeField] private int _startOrder;
        [SerializeField] private int _orderInterval;
        
        private Dictionary<Type, UIPopup> _popupPrefabDict = new();
        private IResourceManager _resourceMgr;


        private void Awake()
        {
            Services.Register<IPopupManager>(this);
        }


        protected override async UniTask<bool> OnInitAsync(PreInitContext ctx)
        {
            _resourceMgr = Services.Get<IResourceManager>();
            
            var resourceInit = _resourceMgr as IPreInitializable;
            if (resourceInit == null)
                return false;
            
            var result = await ctx.GetAsync(resourceInit);
            if (result == null) 
                return false;
            

            _camera = GetComponentInChildren<Camera>();
            if (_camera == null)
                return false;
            
            _camera.cullingMask = LayerMask.GetMask("UI");
            _camera.clearFlags = CameraClearFlags.Depth;

            var prefabs  = _resourceMgr.GetAll<UIPopup>("");
            
            foreach (var prefab in prefabs)
            {
                _popupPrefabDict[prefab.GetType()] = prefab;
            }

            UpdateCameraStack();
            
            
            SceneManager.sceneLoaded += ( scene, mode) =>
            {
                CloseAll();
                UpdateCameraStack();
            };


            return true;
        }



        protected override T OnOpen<T>()
        {
            var opened = Get<T>();
            if (opened != null)
                return Open(opened) as T;
            
            var type = typeof(T);
            
            if (!_popupCache.TryGetValue(type, out UIPopup popup))
            {
                _popupPrefabDict.TryGetValue(type, out UIPopup resource);
                if (resource == null)
                    return null;

                popup = Instantiate(resource);
                popup.Init();

                if (popup.IsGlobal)
                {
                    popup.transform.SetParent(transform);
                }
            }

            return Open(popup) as T;
        }

        protected override void OnClose()
        {
            if (_openedPopups.Count == 0)
                return;

            int lastIndex = _openedPopups.Count - 1;
            var popup = _openedPopups[lastIndex];

            _openedPopups.RemoveAt(lastIndex);
            popup.Close();

            UpdateOrders();
        }

        protected override void OnClose<T>()
        {
            for (int i = _openedPopups.Count - 1; i >= 0; i--)
            {
                if (_openedPopups[i] is T popup)
                {
                    int index = _openedPopups.IndexOf(popup);
                    if (index < 0)
                        return;

                    _openedPopups.RemoveAt(index);
                    
                    popup.Close();
                    UpdateOrders();
                    return;
                }
            }
        }

        private UIPopup Open(UIPopup popup)
        {
            if (_openedPopups.Contains(popup))
            {
                int index = _openedPopups.IndexOf(popup);
                if (index == _openedPopups.Count - 1)
                    return popup;

                _openedPopups.RemoveAt(index);
                _openedPopups.Add(popup);
            }
            else
            {
                _openedPopups.Add(popup);
                popup.SetCamera(_camera);
                popup.Open();
            }
            
            UpdateOrders();
            
            return popup;
        }
        
        private void UpdateOrders()
        {
            for (int i = 0; i < _openedPopups.Count; i++)
            {
                int order = _startOrder + (i + 1) * _orderInterval;
                
                _openedPopups[i].SetOrder(order);
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