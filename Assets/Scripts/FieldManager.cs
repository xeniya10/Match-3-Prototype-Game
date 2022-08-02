using UnityEngine;

public class FieldManager : MonoBehaviour
{
    public int ColumnNumber = 5;
    public int RowNumber = 8;
    public int Padding = 100;
    public int Spacing = 20;
    public float CellSide = 0;
    [HideInInspector] public float CellsOffset = 0;
    [HideInInspector] public float StartXPosition = 0;
    [HideInInspector] public float StartYPosition = 0;

    public void CalculateFieldOptions()
    {
        CellSide = (Screen.width - 2 * Padding - (ColumnNumber - 1) * Spacing) / ColumnNumber;
        CellsOffset = CellSide + Spacing;

        StartXPosition = -Screen.width / 2 + CellSide / 2 + Padding;
        StartYPosition = CellSide / 2 + Padding;
    }
}
