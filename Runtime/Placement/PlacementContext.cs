using UnityEngine;

namespace LLib
{
    public struct PlacementContext
    {
        public Bounds WorldBounds;
        public BoundsInt CellBounds;
        public bool CanPlace;
        public bool IsReplace;
    }
}