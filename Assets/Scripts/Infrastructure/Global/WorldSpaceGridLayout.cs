using UnityEngine;

public class WorldSpaceGridLayout : MonoBehaviour
{
    public enum GridAxis { XZ, XY, YZ }

    [Header("Settings")]
    [SerializeField] private GridAxis _axis = GridAxis.XZ; 
    [SerializeField] private int _columns = 5;            
    [SerializeField] private Vector2 _cellSize = new Vector2(1f, 1f);
    [SerializeField] private Vector2 _spacing = new Vector2(0.1f, 0.1f);

    [Header("Alignment")]
    [SerializeField] private bool _centerGrid = false; 

    void Update()
    {
        ArrangeGrid();
    }

    public void ArrangeGrid()
    {
        int childCount = transform.childCount;
        if (childCount == 0) return;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (!child.gameObject.activeSelf) continue;

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
}
