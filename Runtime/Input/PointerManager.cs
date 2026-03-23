using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


namespace LumosLib
{
    public class PointerManager : MonoBehaviour, IPointerManager, IPreInitializable
    {
        [SerializeField] private InputActionReference _posInputReference;
        [SerializeField] private InputActionReference _clickInputReference;

        private Camera _camera;
        private Vector2 _worldPosition;

        
        public bool IsPressed { get; private set; }
        public Vector2 ScreenPosition { get; private set; }
        public Vector2 WorldPosition { get; private set; }
        
        private Camera Camera
        {
            get
            {
                if (_camera == null)
                {
                    _camera = Camera.main;
                }
                return _camera;
            }
        }

        
        public event UnityAction<PointerDownEvent> OnPointerDown;
        public event UnityAction<PointerUpEvent> OnPointerUp;


        private void Awake()
        {
            Services.Register<IPointerManager>(this);
        }


        private void Update()
        {
            ScreenPosition = _posInputReference.action.ReadValue<Vector2>();
            
            if (Camera == null)
            {
                DebugUtil.LogWarning("Camera is null", "FAIL");
                WorldPosition = Vector2.zero;
            }
            else
            {
                WorldPosition = Camera.ScreenToWorldPoint(ScreenPosition);
            }

            IsPressed = _clickInputReference.action.IsPressed();
                
            if (_clickInputReference.action.WasPressedThisFrame())
            {
                OnPointerDown?.Invoke(
                    new PointerDownEvent(ScreenPosition, WorldPosition, GetHitCollider()));
            }

            if (_clickInputReference.action.WasReleasedThisFrame())
            {
                OnPointerUp?.Invoke(
                    new PointerUpEvent(ScreenPosition, WorldPosition, GetHitCollider()));
            }
        }

        
        public UniTask<bool> InitAsync(PreInitContext ctx)
        {
            if (_clickInputReference == null ||
                _posInputReference == null)
            {
                gameObject.SetActive(false);
                return UniTask.FromResult(false);
            }
            
            _clickInputReference.action.actionMap.Enable(); 
            _posInputReference.action.actionMap.Enable(); 
            
            return UniTask.FromResult(true);
        }


        public Collider2D GetHitCollider()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return null;
            
            return Physics2D.OverlapPoint(WorldPosition);
        }
        
        public void SetCamera(Camera cam)
        {
            _camera = cam;
        }

      
    }
}