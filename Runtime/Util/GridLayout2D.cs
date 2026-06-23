using TriInspector;
using UnityEngine;

[ExecuteAlways]
public class GridLayout2D : MonoBehaviour
{
    [Title("Parameter")] [SerializeField] private StartAxis _sortAxis;

    [SerializeField] private SortDirection _sortDirection;
    [SerializeField] [Min(1)] private int _columns = 1;
    [SerializeField] private Vector2 _spacing;


    private void Update()
    {
        SortGrid();
    }

    private void SortGrid()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            int row, col;


            if (_sortAxis == StartAxis.Horizontal)
            {
                row = i / _columns;
                col = i % _columns;
            }
            else
            {
                col = i / _columns;
                row = i % _columns;
            }

            var mult = Vector2.one;

            switch (_sortDirection)
            {
                case SortDirection.TopRight:
                    mult = new Vector2(1, 1);
                    break;

                case SortDirection.BottomRight:
                    mult = new Vector2(1, -1);
                    break;
                case SortDirection.TopLeft:
                    mult = new Vector2(-1, 1);
                    break;

                case SortDirection.BottomLeft:
                    mult = new Vector2(-1, -1);
                    break;
            }

            var position = new Vector2(col * _spacing.x, row * _spacing.y) * mult;

            transform.GetChild(i).localPosition = position;
        }
    }

    private enum SortDirection
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    private enum StartAxis
    {
        Horizontal,
        Vertical
    }
}