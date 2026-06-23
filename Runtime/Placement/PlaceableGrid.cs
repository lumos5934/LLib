using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace LLib
{
    public class PlaceableGrid : MonoBehaviour
    {
        [PropertySpace(15f)] [Title("Grid")] [SerializeField]
        private Vector2Int _min;

        [SerializeField] private Vector2Int _max;
        [SerializeField] [Min(0)] private float _cellSize;


        [PropertySpace(15f)] [Title("Obstacle")] [SerializeField] [LabelText("Layer")]
        private LayerMask _obstacleLayer;

        [SerializeField] [LabelText("Collision Padding")] [Min(0)]
        private float _obstacleCollisionPadding;


        [PropertySpace(15f)] [Title("Gizmo")] [SerializeField] [LabelText("Show")]
        private bool _showGizmo;

        [SerializeField] [LabelText("Line Color")]
        private Color _gizmoLineColor = new(1, 1, 1, 1);

        [SerializeField] [LabelText("Placeable Color")]
        private Color _gizmoPlaceableColor = new(0, 1, 0, 0.25f);

        private readonly Dictionary<IPlaceable, BoundsInt> _boundsByPlaceable = new();
        private readonly HashSet<Vector2Int> _obstacleCells = new();
        private readonly HashSet<Vector2Int> _occupiedCells = new();


        private BoundsInt _cellBounds;


        public BoundsInt CellBounds => _cellBounds;
        public float CellSize => _cellSize;


        private void Awake()
        {
            UpdateBounds();
        }


        private void LateUpdate()
        {
            var tempMax = _max;
            var tempMin = _min;

            if (tempMax != _max || tempMin != _min) UpdateBounds();
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_showGizmo ||
                _cellSize <= 0)
                return;

            for (var x = CellBounds.min.x; x < CellBounds.max.x; x++)
            for (var y = CellBounds.min.y; y < CellBounds.max.y; y++)
            {
                var cell = new Vector2Int(x, y);

                var worldMin = (Vector2)transform.position + (Vector2)cell * _cellSize;
                var worldCenter = worldMin + Vector2.one * _cellSize * 0.5f;

                Gizmos.color = _gizmoLineColor;
                Gizmos.DrawWireCube(worldCenter, Vector3.one * _cellSize);

                if (Application.isPlaying)
                    if (!_occupiedCells.Contains(cell))
                    {
                        Gizmos.color = _gizmoPlaceableColor;
                        Gizmos.DrawCube(worldCenter, Vector3.one * _cellSize);
                    }
            }
        }
#endif


        public bool Place(IPlaceable placeable, BoundsInt cellBounds)
        {
            foreach (var cell in cellBounds.allPositionsWithin)
                if (!_cellBounds.Contains(cell) || IsOccupied((Vector2Int)cell))
                    return false;

            foreach (var cell in cellBounds.allPositionsWithin) _occupiedCells.Add((Vector2Int)cell);

            _boundsByPlaceable[placeable] = cellBounds;

            return true;
        }


        public bool Remove(IPlaceable placeable)
        {
            if (!_boundsByPlaceable.TryGetValue(placeable, out var cellBounds))
                return false;


            foreach (var pos in cellBounds.allPositionsWithin) _occupiedCells.Remove((Vector2Int)pos);

            _boundsByPlaceable.Remove(placeable);

            return true;
        }


        public bool Occupy(BoundsInt cellBounds)
        {
            foreach (var cell in cellBounds.allPositionsWithin)
                if (!_cellBounds.Contains(cell) || IsOccupied((Vector2Int)cell))
                    return false;

            foreach (var cell in cellBounds.allPositionsWithin) _occupiedCells.Add((Vector2Int)cell);

            return true;
        }


        public void Release(BoundsInt cellBounds)
        {
            foreach (var pos in cellBounds.allPositionsWithin) _occupiedCells.Remove((Vector2Int)pos);
        }


        public bool IsPlaced(IPlaceable placeable)
        {
            return _boundsByPlaceable.ContainsKey(placeable);
        }


        public bool IsPlaced(IPlaceable placeable, out BoundsInt bounds)
        {
            return _boundsByPlaceable.TryGetValue(placeable, out bounds);
        }


        public bool IsOccupied(Vector2Int cell)
        {
            return _occupiedCells.Contains(cell);
        }


        public void SetGridSize(Vector2Int min, Vector2Int max)
        {
            _min = min;
            _max = max;

            UpdateBounds();
        }


        public void SetCellSize(float cellSize)
        {
            _cellSize = cellSize < 0 ? 0 : cellSize;
        }


        public void Scan(Bounds scanBounds)
        {
            var min = Vector3Int.FloorToInt(scanBounds.min / _cellSize);
            var max = Vector3Int.CeilToInt(scanBounds.max / _cellSize);
            var size = max - min;
            size.z = 1;

            var scanCellBounds = new BoundsInt(min, size);

            HashSet<Vector2Int> releaseCells = new();

            foreach (Vector2Int scanCell in scanCellBounds.allPositionsWithin)
                if (_obstacleCells.Contains(scanCell))
                    releaseCells.Add(scanCell);

            _occupiedCells.ExceptWith(releaseCells);
            _obstacleCells.ExceptWith(releaseCells);


            var hitColliders = Physics2D.OverlapBoxAll(scanBounds.center, scanBounds.size, 0, _obstacleLayer);

            foreach (var col in hitColliders)
            {
                var worldBounds = col.bounds;
                worldBounds.Expand(-_obstacleCollisionPadding);

                var hitMin = Vector3Int.FloorToInt(worldBounds.min / _cellSize);
                var hitMax = Vector3Int.CeilToInt(worldBounds.max / _cellSize);
                var hitSize = hitMax - hitMin;
                hitSize.z = 1;

                var obstacleCellBounds = new BoundsInt(hitMin, hitSize);

                foreach (var cell in obstacleCellBounds.allPositionsWithin)
                    if (_cellBounds.Contains(cell))
                    {
                        _occupiedCells.Add((Vector2Int)cell);
                        _obstacleCells.Add((Vector2Int)cell);
                    }
            }
        }


        private void UpdateBounds()
        {
            var width = _max.x - _min.x;
            var height = _max.y - _min.y;

            var size = new Vector3Int(width, height, 1);
            _cellBounds = new BoundsInt((Vector3Int)_min, size);
        }
    }
}