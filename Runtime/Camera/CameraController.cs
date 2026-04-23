using Unity.Cinemachine;
using UnityEngine;


namespace LLib
{
    [RequireComponent(typeof(CinemachineCamera))]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private string _key;
    
        protected CinemachineCamera _camera;
    
        public CinemachineCamera Camera => _camera;

    
        protected virtual void Awake()
        {
            _camera = GetComponent<CinemachineCamera>();
        }

    
        protected virtual void OnEnable()
        {
            Services.Get<CameraManager>()?.Add(_key, this);
        }

    
        protected virtual void OnDisable()
        {
            Services.Get<CameraManager>()?.Remove(_key);
        }
    

        public virtual void OnEnter()
        {
        }

    
        public virtual void OnUpdate()
        {
        }

    
        public virtual void OnExit()
        {
        }
    }
}

