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
    private Vector2[] hitDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private List<Crystal> crystalPool = new List<Crystal>();
    private List<Crystal> selectedCrystals = new List<Crystal>();
    public event Action<int, int> ClearMatchEvent;
    public event Action MoveMadeEvent;

    public void CreatePool()
    {
        gridManager.CalculateFieldParameters();
        crystalPrefab.SetSize(gridManager.CellSize);

        for (int i = 0; i < gridManager.ColumnNumber; i++)
        {
            for (int j = 0; j < gridManager.RowNumber; j++)
            {
                Crystal crystal = crystalPrefab.Create(transform);
                crystalPool.Add(crystal);

                float x = gridManager.StartPosition.x + (gridManager.CellOffset * i);
                float y = gridManager.StartPosition.y + (gridManager.CellOffset * j);
                crystal.SetPosition(x, y);

                crystal.ClickEvent += (selectedCrystal) => Swap(selectedCrystal);
                crystal.DisappearEvent += (clearedCrystal) => ShiftDown(clearedCrystal);
            }
        }
    }

    public void ResetField()
    {
        for (int i = 0; i < crystalPool.Count; i++)
        {
            crystalPool[i].SetRandomType();
        }
    }

    private void Swap(Crystal selectedCrystal)
    {
        selectedCrystals.Add(selectedCrystal);

        if (selectedCrystals.Count < 2)
        {
            return;
        }

        var startPos1 = selectedCrystals[0].transform.localPosition;
        var startPos2 = selectedCrystals[1].transform.localPosition;

        for (int i = 0; i < hitDirections.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(selectedCrystal.transform.position, hitDirections[i]);

            if (hit.collider != null && selectedCrystals.Contains(hit.collider.GetComponent<Crystal>()))
            {
                StartCoroutine(SwapAnimation(startPos1, startPos2));
                return;
            }
        }

        ClearSelectedCrystals();
    }

    private IEnumerator SwapAnimation(Vector3 startPos1, Vector3 startPos2)
    {
        var swap1 = selectedCrystals[0].transform.DOLocalMove(startPos2, 0.2f);
        var swap2 = selectedCrystals[1].transform.DOLocalMove(startPos1, 0.2f);
        yield return swap2.WaitForCompletion();

        if (!FindMatch())
        {
            var undoSwap1 = selectedCrystals[0].transform.DOLocalMove(startPos1, 0.2f);
            var undoSwap2 = selectedCrystals[1].transform.DOLocalMove(startPos2, 0.2f);
            yield return undoSwap2.WaitForCompletion();
        }

        else
        {
            MoveMadeEvent?.Invoke();
        }

        ClearSelectedCrystals();
    }

    private void ClearSelectedCrystals()
    {
        foreach (var crystal in selectedCrystals)
        {
            crystal.Deselect();
        }

        selectedCrystals.Clear();
    }

    public bool FindMatch()
    {
        List<Crystal> matchingBalls = new List<Crystal>();
        bool IsMatched = false;

        for (int i = 0; i < crystalPool.Count; i++)
        {
            for (int j = 0; j < hitDirections.Length; j++)
            {
                RaycastHit2D hit = Physics2D.Raycast(crystalPool[i].transform.position, hitDirections[j]);

                if (hit.collider?.GetComponent<Crystal>().CrystalType == crystalPool[i].CrystalType)
                {
                    hit.collider.GetComponent<Crystal>().CheckNeighbor(matchingBalls, hitDirections[j]);
                    matchingBalls.Add(crystalPool[i]);
                }

                if (matchingBalls.Count > 2)
                {
                    matchingBalls = matchingBalls.OrderByDescending(ball => ball.transform.position.y).ToList();

                    foreach (var matchingBall in matchingBalls)
                    {
                        matchingBall.Disappear();
                    }

                    ClearMatchEvent?.Invoke(matchingBalls[0].CrystalType, matchingBalls.Count);
                    IsMatched = true;
                }
                matchingBalls.Clear();
            }
        }

        return IsMatched;
    }

    public void ShiftDown(Crystal clearedCrystal)
    {
        List<Crystal> crystalsAbove = FindCrystalsAbove(clearedCrystal);

        if (crystalsAbove.Count == 0)
        {
            clearedCrystal.ShowUp(() =>
            {
                if (!FindMatch())
                {
                    CheckPossibleMove();
                }
            });
            return;
        }

        crystalsAbove = crystalsAbove.OrderBy(ball => ball.transform.position.y).ToList();

        foreach (var crystal in crystalsAbove)
        {
            float shiftedYPosition = gridManager.CalculateShiftedYPosition(crystal, -1);
            crystal.SetPosition(crystal.transform.localPosition.x, shiftedYPosition);
        }

        Crystal topColumnCrystal = crystalsAbove[crystalsAbove.Count - 1];
        ShiftUp(clearedCrystal, topColumnCrystal);
    }

    private List<Crystal> FindCrystalsAbove(Crystal clearedCrystal)
    {
        List<Crystal> crystalsAbove = new List<Crystal>();

        for (int i = 0; i < crystalPool.Count; i++)
        {
            Vector2 crystalPos = crystalPool[i].transform.localPosition;
            Vector2 clearedCrystalPos = clearedCrystal.transform.localPosition;

            if (crystalPos.x == clearedCrystalPos.x && crystalPos.y > clearedCrystalPos.y)
            {
                crystalsAbove.Add(crystalPool[i]);
            }
        }

        return crystalsAbove;
    }

    private void ShiftUp(Crystal clearedCrystal, Crystal topCrystal)
    {
        var topCrystalRowNumber = gridManager.CalculateRowNumber(topCrystal);
        var clearedCrystalRowNumber = gridManager.CalculateRowNumber(clearedCrystal);

        var rowNumberShift = topCrystalRowNumber - clearedCrystalRowNumber + 1;
        var shiftedYPosition = gridManager.CalculateShiftedYPosition(clearedCrystal, rowNumberShift);

        clearedCrystal.SetPosition(clearedCrystal.transform.localPosition.x, shiftedYPosition);
        clearedCrystal.ShowUp(() =>
            {
                if (!FindMatch())
                {
                    CheckPossibleMove();
                }
            });
    }

    private void CheckPossibleMove()
    {
        List<Crystal> matchingBalls = new List<Crystal>();

        for (int i = 0; i < crystalPool.Count; i++)
        {
            for (int j = 0; j < hitDirections.Length - 2; j++)
            {
                for (int k = j + 1; k < hitDirections.Length; k++)
                {
                    RaycastHit2D firstHit = Physics2D.Raycast(crystalPool[i].transform.position, hitDirections[j]);
                    RaycastHit2D secondHit = Physics2D.Raycast(crystalPool[i].transform.position, hitDirections[k]);

                    if (firstHit.collider != null && secondHit.collider != null &&
                        firstHit.collider?.GetComponent<Crystal>().CrystalType == secondHit.collider?.GetComponent<Crystal>().CrystalType)
                    {
                        firstHit.collider.GetComponent<Crystal>().CheckNeighbor(matchingBalls, hitDirections[j]);
                        secondHit.collider.GetComponent<Crystal>().CheckNeighbor(matchingBalls, hitDirections[k]);
                    }
                }

                if (matchingBalls.Count > 2)
                {
                    return;
                }
                matchingBalls.Clear();
            }
        }
        ResetField();
    }
}
