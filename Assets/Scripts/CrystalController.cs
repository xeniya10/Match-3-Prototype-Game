using System;
using System.Collections.Generic;
using UnityEngine;

public class CrystalController : MonoBehaviour
{
    [SerializeField] private Crystal crystalPrefab;
    [SerializeField] private FieldManager fieldManager;
    private int totalCrystalNumber;
    private float searchMatchDelay = 0.5f;
    private DateTime searchMatchTimer;
    private Vector2[] matchDirections = { Vector2.right, Vector2.down };
    private List<Crystal> crystalPool = new List<Crystal>();
    public event Action<int> MatchEvent;
    public event Action CheckGameOverEvent;

    public void CreatePool()
    {
        fieldManager.CalculateFieldOptions();
        crystalPrefab.SetSize(fieldManager.CellSide);

        for (int i = 0; i < fieldManager.ColumnNumber; i++)
        {
            for (int j = 0; j < fieldManager.RowNumber; j++)
            {
                Crystal crystal = crystalPrefab.Create(transform);
                float xPos = fieldManager.StartXPosition + (fieldManager.CellsOffset * i);
                float yPos = fieldManager.StartYPosition + (fieldManager.CellsOffset * j);
                crystal.transform.localPosition = new Vector2(xPos, yPos);
                crystal.CrystalClickEvent += () => MatchEvent?.Invoke(-1);
                crystalPool.Add(crystal);
            }
        }

        searchMatchTimer = DateTime.Now;
    }

    // public void FindMatch()
    // {
    //     if (DateTime.Compare(DateTime.Now, _searchMatchTimer.AddSeconds(_searchMatchDelay)) >= 0)
    //     {
    //         int matchingCounter = 0;

    //         for (int i = 0; i < _ballPool.Count; i++)
    //             CheckMatchInOneDirection(_ballPool[i], ref matchingCounter);

    //         _searchMatchTimer = DateTime.Now;

    //         if (matchingCounter == 0)
    //         {
    //             CheckMovesOverEvent?.Invoke();
    //             ShowFirstMove();
    //             CheckPossibleMoves();
    //         }
    //     }
    // }

    // private void CheckMatchInOneDirection(Ball ball, ref int matchingCounter)
    // {
    //     List<Ball> matchingBalls = new List<Ball>();

    //     for (int j = 0; j < _matchDirections.Length; j++)
    //     {
    //         RaycastHit2D hit = Physics2D.Raycast(ball.transform.position, _matchDirections[j]);

    //         if (hit.collider?.GetComponent<Ball>().ColorNumber == ball.ColorNumber)
    //         {
    //             hit.collider.GetComponent<Ball>().CheckNeighbor(matchingBalls, _matchDirections[j]);
    //             matchingBalls.Add(ball);
    //         }

    //         if (matchingBalls.Count > 2)
    //         {
    //             matchingCounter++;
    //             matchingBalls = matchingBalls.OrderByDescending(ball => ball.transform.position.y).ToList();

    //             foreach (var matchingBall in matchingBalls)
    //                 matchingBall.Burst(null);

    //             ChangeMovesEvent?.Invoke(matchingBalls.Count);
    //         }
    //         matchingBalls.Clear();
    //     }
    // }
}
