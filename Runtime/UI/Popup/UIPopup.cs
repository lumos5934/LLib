using UnityEngine;

namespace LLib
{
    public abstract class UIPopup : UIScene
    {
        [SerializeField] private bool _isModal = true;

        public bool IsModal => _isModal;
    }
}