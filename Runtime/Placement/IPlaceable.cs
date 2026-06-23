using UnityEngine;

namespace LLib
{
    public interface IPlaceable
    {
        GameObject gameObject { get; }

        Vector2Int Size { get; }

        void OnBegin();
        void OnPlace();
        void OnRemove();
        void OnCancel();
        void OnUpdated(PlacementContext context);
    }
}