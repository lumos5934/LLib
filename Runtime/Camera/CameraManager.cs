using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;


namespace LLib
{
    public class CameraManager : MonoBehaviour
    {
        private readonly Dictionary<string, CameraController> _controllersByKey = new();

        private int _curPriority;
        private Camera _main;
        private CameraController _activeController;

        public Camera Main
        {
            get
            {
                if (_main == null)
                {
                    _main = Camera.main;
                }
                
                return _main;
            }
        }

        
        private void Awake()
        {
            Services.Register(this);
            
            CinemachineCore.CameraActivatedEvent.AddListener(OnCameraActivated);
        }


        private void OnDestroy()
        {
            CinemachineCore.CameraActivatedEvent.RemoveListener(OnCameraActivated);
        }


        private void Update()
        {
            _activeController?.OnUpdate();
        }


        internal void Add(string key, CameraController controller)
        {
            if (_controllersByKey.TryAdd(key, controller))
            {
                var priority = controller.Camera.Priority;
                
                if (priority > _curPriority)
                {
                    _curPriority = priority;
                }
            }
        }

        
        internal void Remove(string key)
        {
            _controllersByKey.Remove(key);
        }


        public CameraController GetCameraController(string key)
        {
            return  _controllersByKey.GetValueOrDefault(key);
        }
        
        
        public CameraController Switch(string key)
        {
            if (_controllersByKey.TryGetValue(key, out var controller))
            {
                controller.Camera.Priority =  ++_curPriority;
                
                return controller;
            }

            return null;
        }
        
        
        private void OnCameraActivated(ICinemachineCamera.ActivationEventParams eventParams)
        {
            if (eventParams.Origin is MonoBehaviour oldMono)
            {
                var oldController = oldMono.GetComponent<CameraController>();

                if (oldController != null)
                {
                    _activeController = oldController;
                    _activeController.OnExit();
                }
            }

            if (eventParams.IncomingCamera is MonoBehaviour newMono)
            {
                var newController = newMono.GetComponent<CameraController>();
                if (newController != null)
                {
                    _activeController = newController;
                    _activeController.OnExit();
                }
            }
        }
    }
}

