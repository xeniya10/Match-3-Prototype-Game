using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3Prototype
{
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
        [SerializeField] private CrystalSprites spriteContainer;

        [Header("Game Result")]
        [SerializeField] private string winText = "WIN";
        [SerializeField] private string loseText = "Game Over";

        public event Action openGameScreenEvent;
        public event Action openMenuScreenEvent;
        public event Action openGameOverScreenEvent;
        public event Action clickExitButtonEvent;

        public void OpenMenuScreen()
        {
            gameScreen.SetActive(false);
            menuScreen.SetActive(true);
            gameOverScreen.SetActive(false);

            openMenuScreenEvent?.Invoke();
        }

        public void OpenGameScreen()
        {
            gameScreen.SetActive(true);
            menuScreen.SetActive(false);
            gameOverScreen.SetActive(false);

            openGameScreenEvent?.Invoke();
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
            Sprite sprite = spriteContainer.sprites[spriteNumber];
            targetCrystalImage.sprite = sprite;
        }

        public void OpenGameOverScreen(int targetNumbers, int movesNumbers)
        {
            SetGameResult(targetNumbers, movesNumbers);
            gameOverScreen.SetActive(true);

            openGameOverScreenEvent?.Invoke();
        }

        private void SetGameResult(int targetNumbers, int movesNumbers)
        {
            if (targetNumbers < 1)
            {
                gameResultText.text = winText;
                return;
            }

            if (movesNumbers < 1)
            {
                gameResultText.text = loseText;
            }
        }

        public void OnClickExitGame()
        {
            clickExitButtonEvent?.Invoke();
        }
    }
}
