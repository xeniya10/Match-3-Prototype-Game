using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int ColumnNumber = 5;
    public int RowNumber = 8;
    public int Padding = 100;
    public int Spacing = 20;
    public float CellSize = 0;
    [HideInInspector] public float CellOffset = 0;
    [HideInInspector] public Vector2 StartPosition;

    public void CalculateFieldParameters()
    {
        CalculateCellParameters();
        CalculateStartPosition();
    }

    private void CalculateCellParameters()
    {
        int paddingNumber = 2;
        int spacingNumber = ColumnNumber - 1;

        float cellSpace = Screen.width - paddingNumber * Padding - spacingNumber * Spacing;
        CellSize = cellSpace / ColumnNumber;
        CellOffset = CellSize + Spacing;
    }

    private void CalculateStartPosition()
    {
        StartPosition = new Vector2();

        StartPosition.x = -Screen.width / 2 + CellSize / 2 + Padding;
        StartPosition.y = CellSize / 2 + Padding;
    }

    public float CalculateRowNumber(Crystal crystal)
    {
        float rowNumber = 1 + (crystal.transform.localPosition.y - StartPosition.y) / CellOffset;
        return rowNumber;
    }

    public float CalculateShiftedYPosition(Crystal crystal, float rowShiftNumber)
    {
        float shiftedYPosition = crystal.transform.localPosition.y + CellOffset * rowShiftNumber;
        return shiftedYPosition;
    }
}
