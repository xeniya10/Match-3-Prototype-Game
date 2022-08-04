using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject gameScreen;
    [SerializeField] private GameObject gameOverScreen;
    [Space]
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private Image targetCrystalImage;
    [SerializeField] private TextMeshProUGUI gameResultText;
    [SerializeField] private CrystalSprites crystalSprites;
    [Header("Game Result")]
    [SerializeField] private string winText = "WIN";
    [SerializeField] private string loseText = "Game Over";

    public event Action OpenGameScreenEvent;
    public event Action OpenMenuScreenEvent;
    public event Action OpenGameOverScreenEvent;
    public event Action ClickExitButtonEvent;

    public void OpenMenuScreen()
    {
        gameScreen.SetActive(false);
        menuScreen.SetActive(true);
        gameOverScreen.SetActive(false);

        OpenMenuScreenEvent?.Invoke();
    }

    public void OpenGameScreen()
    {
        gameScreen.SetActive(true);
        menuScreen.SetActive(false);
        gameOverScreen.SetActive(false);

        OpenGameScreenEvent?.Invoke();
    }

    public void SetMovesNumber(int number)
    {
        movesText.text = number.ToString();
    }

    public void SetTargetNumber(int number)
    {
        targetText.text = number.ToString();
    }

    public void SetTargetSprite(int spriteNumber)
    {
        Sprite sprite = crystalSprites.Sprites[spriteNumber];
        targetCrystalImage.sprite = sprite;
    }

    public void OpenGameOver(int crystalsTarget, int moves)
    {
        SetGameResult(crystalsTarget, moves);
        gameOverScreen.SetActive(true);

        OpenGameOverScreenEvent?.Invoke();
    }

    private void SetGameResult(int crystalsTarget, int moves)
    {
        if (crystalsTarget < 1)
        {
            gameResultText.text = winText;
            return;
        }

        if (moves < 1)
        {
            gameResultText.text = loseText;
        }
    }

    public void ExitGame()
    {
        ClickExitButtonEvent?.Invoke();
    }
}
