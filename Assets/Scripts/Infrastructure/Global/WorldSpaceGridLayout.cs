using UnityEngine;

public class WorldSpaceGridLayout : MonoBehaviour
{
    #region Enums

    public enum GridAxis { XZ, XY, YZ }

    #endregion


    #region Fields

    [Header("Settings")]
    [Tooltip("Axis along which the grid is laid out.")]
    [SerializeField] private GridAxis _axis = GridAxis.XZ;
    [Tooltip("Number of columns in the grid.")]
    [SerializeField] private int _columns = 5;
    [Tooltip("Size of each cell.")]
    [SerializeField] private Vector2 _cellSize = new Vector2(1f, 1f);
    [Tooltip("Spacing between cells.")]
    [SerializeField] private Vector2 _spacing = new Vector2(0.1f, 0.1f);

    [Header("Alignment")]
    [Tooltip("Center the grid relative to parent.")]
    [SerializeField] private bool _centerGrid = false;

    #endregion


    #region Public Methods

    public void ArrangeGrid()
    {
        int childCount = transform.childCount;
        if (childCount == 0)
        {
            return;
        }

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (!child.gameObject.activeSelf)
            {
                continue;
            }

            int row = i / _columns;
            int col = i % _columns;

            float posX = col * (_cellSize.x + _spacing.x);
            float posY = row * (_cellSize.y + _spacing.y);

            if (_centerGrid)
            {
                int totalRows = Mathf.CeilToInt((float)childCount / _columns);
                int curCols = Mathf.Min(childCount, _columns);

                posX -= (curCols - 1) * (_cellSize.x + _spacing.x) / 2f;
                posY -= (totalRows - 1) * (_cellSize.y + _spacing.y) / 2f;
            }

            switch (_axis)
            {
                case GridAxis.XZ:
                    child.localPosition = new Vector3(posX, child.localPosition.y, -posY);
                    break;
                case GridAxis.XY:
                    child.localPosition = new Vector3(posX, -posY, 0);
                    break;
                case GridAxis.YZ:
                    child.localPosition = new Vector3(0, -posY, posX);
                    break;
            }
        }
    }

    #endregion


    #region Unity Methods

    private void Update()
    {
        ArrangeGrid();
    }

    #endregion
}