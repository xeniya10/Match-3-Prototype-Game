using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class CrystalController : MonoBehaviour
{
    [SerializeField] private Crystal crystalPrefab;
    [SerializeField] private GridManager gridManager;
    private Vector2[] raycastDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private List<Crystal> crystalsPool = null;
    private List<Crystal> clickedCrystals = null;

    /// <summary>Called, when matched crystals were cleared, and give color and number of cleared crystals.</summary>
    public event Action<CrystalColor, int> matchedCrystalsClearedEvent;
    public event Action swapEvent;

    public void CreatePool()
    {
        gridManager.CalculateGridParameters();
        crystalPrefab.SetSize(gridManager.cellSize);

        crystalsPool = new List<Crystal>();

        for (int i = 0; i < gridManager.columnNumber; i++)
        {
            for (int j = 0; j < gridManager.rowNumber; j++)
            {
                Crystal crystal = crystalPrefab.Create(transform);
                crystalsPool.Add(crystal);

                float x = gridManager.startCellPosition.x + (gridManager.cellOffset * i);
                float y = gridManager.startCellPosition.y + (gridManager.cellOffset * j);
                crystal.SetPosition(x, y);

                crystal.clickEvent += (clickedCrystal) => Swap(clickedCrystal);
            }
        }
    }

    private void Swap(Crystal clickedCrystal)
    {
        if (clickedCrystals == null)
        {
            clickedCrystals = new List<Crystal>();
        }

        clickedCrystals.Add(clickedCrystal);

        if (clickedCrystals.Count < 2)
        {
            return;
        }

        Vector2 startPosition1 = clickedCrystals[0].transform.localPosition;
        Vector2 startPosition2 = clickedCrystals[1].transform.localPosition;

        // Check by raycast clicked crystals are neighbors or not
        for (int i = 0; i < raycastDirections.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(clickedCrystal.transform.position, raycastDirections[i]);

            if (hit.collider != null && clickedCrystals.Contains(hit.collider.GetComponent<Crystal>()))
            {
                StartCoroutine(SwapAnimation(startPosition1, startPosition2));
                return;
            }
        }

        foreach (Crystal crystal in clickedCrystals)
        {
            crystal.SetBorderActive(false);
        }

        clickedCrystals.Clear();
    }

    private IEnumerator SwapAnimation(Vector3 startPos1, Vector3 startPos2)
    {
        var swapAnimation1 = clickedCrystals[0].transform.DOLocalMove(startPos2, 0.2f);
        var swapAnimation2 = clickedCrystals[1].transform.DOLocalMove(startPos1, 0.2f);

        yield return swapAnimation2.WaitForCompletion();

        // If after swaping doesn't find matching cristals, start reverse swap animation
        if (!FindMatch())
        {
            var undoSwapAnimation1 = clickedCrystals[0].transform.DOLocalMove(startPos1, 0.2f);
            var undoSwapAnimation2 = clickedCrystals[1].transform.DOLocalMove(startPos2, 0.2f);

            yield return undoSwapAnimation2.WaitForCompletion();
        }

        // Else move was made
        else
        {
            swapEvent?.Invoke();
        }

        foreach (Crystal crystal in clickedCrystals)
        {
            crystal.SetBorderActive(false);
        }

        clickedCrystals.Clear();
    }

    /// <summary>Looking for neighbor cristals with the same color by raycasts in two directions (down, left).</summary>
    public bool FindMatch()
    {
        bool isMatchedFound = false;
        List<Crystal> matchingCrystals = new List<Crystal>();

        for (int i = 0; i < crystalsPool.Count; i++)
        {
            for (int j = 1; j < raycastDirections.Length - 1; j++)
            {
                RaycastHit2D raycastHit = Physics2D.Raycast(crystalsPool[i].transform.position, raycastDirections[j]);

                if (raycastHit.collider?.GetComponent<Crystal>().CrystalColor == crystalsPool[i].CrystalColor)
                {
                    raycastHit.collider.GetComponent<Crystal>().CheckNeighbor(matchingCrystals, raycastDirections[j]);
                    matchingCrystals.Add(crystalsPool[i]);
                }

                if (matchingCrystals.Count > 2)
                {
                    // Sorting to start clearing with the highest matched crystal
                    matchingCrystals = matchingCrystals.OrderByDescending(crystal => crystal.transform.position.y).ToList();

                    foreach (Crystal matchingCrystal in matchingCrystals)
                    {
                        matchingCrystal.Clear((clearedCrystal) => Shift(clearedCrystal));
                    }

                    matchedCrystalsClearedEvent?.Invoke(matchingCrystals[0].CrystalColor, matchingCrystals.Count);
                    isMatchedFound = true;
                }
                matchingCrystals.Clear();
            }
        }

        return isMatchedFound;
    }

    /// <summary>First of all, looking for all uncleared crystal located above cleared crystal, then shifted down all of them and shifted up cleared crystal</summary>
    public void Shift(Crystal clearedCrystal)
    {
        List<Crystal> crystalsAbove = FindCrystalsAbove(clearedCrystal);

        if (crystalsAbove.Count == 0)
        {
            clearedCrystal.Refill(() => CheckPossibleMove());
            return;
        }

        // Sorting to start shifting with the lowest crystal
        crystalsAbove = crystalsAbove.OrderBy(crystal => crystal.transform.position.y).ToList();

        foreach (Crystal crystal in crystalsAbove)
        {
            // Shift down one position
            float verticalShiftDown = crystal.transform.localPosition.y - gridManager.cellOffset;
            crystal.SetPosition(crystal.transform.localPosition.x, verticalShiftDown);
        }

        // Shift up one position from the highest crystal above
        Crystal topCrystal = crystalsAbove[crystalsAbove.Count - 1];
        float verticalShiftUp = topCrystal.transform.localPosition.y + gridManager.cellOffset;

        clearedCrystal.SetPosition(clearedCrystal.transform.localPosition.x, verticalShiftUp);
        clearedCrystal.Refill(() => CheckPossibleMove());
    }

    private List<Crystal> FindCrystalsAbove(Crystal clearedCrystal)
    {
        List<Crystal> crystalsAbove = new List<Crystal>();

        for (int i = 0; i < crystalsPool.Count; i++)
        {
            Vector2 crystalPos = crystalsPool[i].transform.localPosition;
            Vector2 clearedCrystalPos = clearedCrystal.transform.localPosition;

            if (crystalPos.x == clearedCrystalPos.x && crystalPos.y > clearedCrystalPos.y)
            {
                crystalsAbove.Add(crystalsPool[i]);
            }
        }

        return crystalsAbove;
    }

    public void CheckPossibleMove()
    {
        if (!FindMatch())
        {
            List<Crystal> matchingCrystals = new List<Crystal>();

            for (int i = 0; i < crystalsPool.Count; i++)
            {
                for (int j = 0; j < raycastDirections.Length - 2; j++)
                {
                    for (int k = j + 1; k < raycastDirections.Length; k++)
                    {
                        RaycastHit2D firstHit = Physics2D.Raycast(crystalsPool[i].transform.position, raycastDirections[j]);
                        RaycastHit2D secondHit = Physics2D.Raycast(crystalsPool[i].transform.position, raycastDirections[k]);

                        if (firstHit.collider != null && secondHit.collider != null &&
                            firstHit.collider?.GetComponent<Crystal>().CrystalColor == secondHit.collider?.GetComponent<Crystal>().CrystalColor)
                        {
                            firstHit.collider.GetComponent<Crystal>().CheckNeighbor(matchingCrystals, raycastDirections[j]);
                            secondHit.collider.GetComponent<Crystal>().CheckNeighbor(matchingCrystals, raycastDirections[k]);
                        }
                    }

                    if (matchingCrystals.Count > 2)
                    {
                        return;
                    }
                    matchingCrystals.Clear();
                }
            }
            ResetField();
        }
    }

    public void ResetField()
    {
        for (int i = 0; i < crystalsPool.Count; i++)
        {
            crystalsPool[i].Clear((crystal) => crystal.Refill(() => FindMatch()));
        }
    }
}
