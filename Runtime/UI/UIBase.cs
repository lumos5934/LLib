using UnityEngine;

namespace LLib
{
    [RequireComponent(typeof(CanvasGroup),
        typeof(RectTransform))]
    public abstract class UIBase : MonoBehaviour
    {
        public bool IsOpened { get; protected set; }
        public CanvasGroup CanvasGroup { get; private set; }
        public RectTransform RectTransform { get; private set; }


        public virtual void Init()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
            RectTransform = GetComponent<RectTransform>();
        }


        public void Open()
        {
            if (IsOpened)
                return;

            IsOpened = true;

            OnOpen();
        }


        public void Close()
        {
            if (!IsOpened)
                return;

            IsOpened = false;

            OnClose();
        }


        public abstract void Refresh();

        protected virtual void OnOpen()
        {
            gameObject.SetActive(true);
        }

        protected virtual void OnClose()
        {
            gameObject.SetActive(false);
        }
    }
}