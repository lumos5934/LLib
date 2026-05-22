using UnityEngine;

namespace LLib
{
    [RequireComponent(typeof(Canvas))]
    public abstract class UIScene : UIBase
    {
        public Canvas Canvas { get; private set; }

        public override void Init()
        {
            base.Init();
            
            Canvas =  GetComponent<Canvas>();
            
            var childUIs = GetComponentsInChildren<UIBase>();
            foreach (var ui in childUIs)
            {
                if (ui.gameObject == gameObject) 
                    continue;
                
                ui.Init();
            }
        }
    }
}