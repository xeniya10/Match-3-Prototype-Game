using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private CrystalController crystalController;
    [SerializeField] private TopBar topBar;
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private TextMeshProUGUI gameResultText;
    [SerializeField] private GameObject gameOverScreen;

    private string winResult = "WIN";
    private string loseResult = "Game Over";

    private int moves = 0;
    private int crystalsTarget = 0;

    private void Awake()
    {
        ShowMenuScreen();
    }

    public void StartGame()
    {
        SetMoves(30);
        SetGameTarget(10);
        ShowGameScreen();
    }

    private void CheckGameOver()
    {
        if (crystalsTarget == 0 || moves == 0)
        {
            ShowGameOverScreen();
        }
    }

    public void ExitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void ShowMenuScreen()
    {
        RunTime();
        gameObject.SetActive(false);
        menuScreen.SetActive(true);
        gameOverScreen.SetActive(false);
    }

    private void ShowGameScreen()
    {
        PauseTime();
        gameObject.SetActive(true);
        menuScreen.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    private void ShowGameOverScreen()
    {
        PauseTime();
        SetGameResult();
        gameOverScreen.SetActive(true);
    }

    private void SetMoves(int number)
    {
        moves = number;
        topBar.SetMovesNumber(moves);
    }

    private void SetGameTarget(int number)
    {
        crystalsTarget = number;
        topBar.SetTargetNumber(crystalsTarget);

        // Sprite crystal = list[Random.Range(0, 8)];
        // topBar.SetTargetCrystal(crystal);
    }

    private void SetGameResult()
    {
        if (crystalsTarget == 0)
        {
            gameResultText.text = winResult;
            return;
        }

        if (moves == 0)
        {
            gameResultText.text = loseResult;
        }
    }

    private void RunTime() => Time.timeScale = 1;

    private void PauseTime() => Time.timeScale = 0;

    private void SubscribeToEvents()
    {

    }
}
