using LLib;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopupDimmer : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        PopupManager.Instance?.Close();
    }
}