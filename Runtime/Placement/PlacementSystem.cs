using UnityEngine;

namespace LLib
{
    public class PlacementSystem
    {
        public enum CellPivot
        {
            Center,
            BottomLeft,
            BottomRight,
            TopLeft,
            TopRight
        }

        private PlacementContext _context;


        public IPlaceable Placeable { get; private set; }

        public PlaceableGrid Grid { get; private set; }

        public PlacementContext Context => _context;
        public CellPivot Pivot { get; private set; } = CellPivot.BottomLeft;


        public void Begin(PlaceableGrid grid, IPlaceable target, Vector3 position)
        {
            if (target == null)
                return;

            if (Placeable != null)
            {
                if (Placeable != target)
                    Placeable.OnCancel();
                else
                    return;
            }

            Grid = grid;
            Placeable = target;


            _context.IsReplace = Grid.IsPlaced(Placeable, out var bounds);

            if (_context.IsReplace)
            {
                Grid.Release(bounds);
            }
            else
            {
                var startCell = CalculateStartCell(position, Placeable.Size);

                bounds = new BoundsInt((Vector3Int)startCell, (Vector3Int)Placeable.Size);
            }

            UpdateContext(bounds);

            Placeable.OnBegin();
        }


        public void Move(Vector3 position)
        {
            if (Grid == null || Placeable == null)
                return;

            var startCell = CalculateStartCell(position, Placeable.Size);

            var newBounds = new BoundsInt((Vector3Int)startCell, _context.CellBounds.size);

            var clampedMinX =
                Mathf.Clamp(newBounds.xMin, Grid.CellBounds.xMin, Grid.CellBounds.xMax - newBounds.size.x);
            var clampedMinY =
                Mathf.Clamp(newBounds.yMin, Grid.CellBounds.yMin, Grid.CellBounds.yMax - newBounds.size.y);

            var clampedNewBounds = new BoundsInt(new Vector3Int(clampedMinX, clampedMinY, 0), newBounds.size);
            if (clampedNewBounds != _context.CellBounds) UpdateContext(clampedNewBounds);
        }


        public bool TryPlace()
        {
            if (Grid == null ||
                Placeable == null)
                return false;


            if (Grid.Place(Placeable, _context.CellBounds))
            {
                Placeable.OnPlace();
                Placeable = null;
                return true;
            }

            return false;
        }


        public bool Remove()
        {
            if (Grid == null ||
                Placeable == null)
                return false;

            if (Grid.Remove(Placeable))
            {
                Placeable.OnRemove();
                Placeable = null;
                Grid = null;

                return true;
            }

            return false;
        }


        public void Cancel()
        {
            if (Grid == null ||
                Placeable == null)
                return;

            if (_context.IsReplace)
            {
                if (Grid.IsPlaced(Placeable, out var bounds)) Grid.Occupy(bounds);

                Placeable.OnCancel();
            }
            else
            {
                Placeable.OnRemove();
            }

            Grid = null;
            Placeable = null;
        }


        public void SetPivot(CellPivot cellPivot)
        {
            Pivot = cellPivot;
        }


        private Vector2Int CalculateStartCell(Vector3 position, Vector2Int size)
        {
            var offset = Vector3.zero;

            if (Pivot == CellPivot.Center)
            {
                if (size.x % 2 == 0) offset.x = 0.5f * Grid.CellSize;
                if (size.y % 2 == 0) offset.y = 0.5f * Grid.CellSize;
            }

            var gridCellBounds = Grid.CellBounds;
            var gridCellSize = Grid.CellSize;

            var cellX = Mathf.FloorToInt(position.x / gridCellSize);
            var cellY = Mathf.FloorToInt(position.y / gridCellSize);

            cellX = Mathf.Clamp(cellX, gridCellBounds.min.x, gridCellBounds.max.x);
            cellY = Mathf.Clamp(cellY, gridCellBounds.min.y, gridCellBounds.max.y);

            var startCell = new Vector2Int(cellX, cellY);


            switch (Pivot)
            {
                case CellPivot.Center:
                    startCell.x -= size.x / 2;
                    startCell.y -= size.y / 2;
                    break;
                case CellPivot.BottomRight:
                    startCell.x -= size.x - 1;
                    break;
                case CellPivot.TopLeft:
                    startCell.y -= size.y - 1;
                    break;
                case CellPivot.TopRight:
                    startCell.x -= size.x - 1;
                    startCell.y -= size.y - 1;
                    break;
                case CellPivot.BottomLeft:
                default:
                    break;
            }

            return startCell;
        }


        private void UpdateContext(BoundsInt cellBounds)
        {
            var worldSize = (Vector3)cellBounds.size * Grid.CellSize;
            var worldMin = (Vector3)cellBounds.min * Grid.CellSize;
            var worldCenter = worldMin + worldSize * 0.5f;
            var worldBounds = new Bounds(worldCenter, worldSize);

            cellBounds.size = new Vector3Int(cellBounds.size.x, cellBounds.size.y, 1);

            _context.WorldBounds = worldBounds;
            _context.CellBounds = cellBounds;

            var canPlace = true;

            foreach (Vector2Int cell in cellBounds.allPositionsWithin)
                if (Grid.IsOccupied(cell))
                {
                    canPlace = false;
                    break;
                }

            _context.CanPlace = canPlace;

            Placeable.OnUpdated(_context);
        }
    }
}