using UnityEngine;

namespace Match3Prototype
{
    public class GridManager : MonoBehaviour
    {
        public int columnNumber = 5;
        public int rowNumber = 8;
        public float cellSize = 0;
        [SerializeField] private int padding = 0;
        [SerializeField] private int spacing = 0;

        /// <summary>Offset between local position of two nearest cells.</summary>
        [HideInInspector] public float cellOffset = 0;
        [HideInInspector] public Vector2 startCellPosition;

        /// <summary>Calculate size and offset of cells, cell start position.</summary>
        public void CalculateGridParameters()
        {
            CalculateMargins();
            CalculateCellParameters();
            CalculateStartCellPosition();
        }

        public void CalculateMargins()
        {
            padding = (int)(Screen.width * 0.1f);
            spacing = (int)(Screen.width * 0.01f);
        }

        private void CalculateCellParameters()
        {
            int paddingNumber = 2;
            int spacingNumber = columnNumber - 1;

            float cellSpace = Screen.width - paddingNumber * padding - spacingNumber * spacing;
            cellSize = cellSpace / columnNumber;
            cellOffset = cellSize + spacing;
        }

        private void CalculateStartCellPosition()
        {
            startCellPosition = new Vector2();

            startCellPosition.x = -Screen.width / 2 + cellSize / 2 + padding;
            startCellPosition.y = cellSize / 2 + padding;
        }
    }
}
