using UnityEngine;

namespace Match3Prototype
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private CrystalController crystalController;
        [SerializeField] private ScreenManager screenManager;
        [SerializeField] private CrystalSprites crystalSprites;
        private int currentMovesNumber = 0;
        private int currentTargetCrystalsNumber = 0;

        [Header("Game start options")]
        public int startMovesNumber = 30;
        public int startTargetCrystalsNumber = 20;
        [SerializeField] private CrystalColor targetCrystalColor = 0;

        private void Awake()
        {
            SubscribeToEvents();
            crystalController.CreatePool();
            screenManager.OpenMenuScreen();
        }

        public void StartGame()
        {
            ResetGameTarget();
            ResetMoves();
            RunTime();
            crystalController.ResetField();
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        private void ResetMoves()
        {
            currentMovesNumber = startMovesNumber;
            screenManager.SetMovesNumber(currentMovesNumber);
        }

        private void UpdateMoves()
        {
            currentMovesNumber = currentMovesNumber - 1;
            screenManager.SetMovesNumber(currentMovesNumber);

            if (currentMovesNumber == 0)
            {
                screenManager.OpenGameOverScreen(currentTargetCrystalsNumber, currentMovesNumber);
            }
        }

        private void ResetGameTarget()
        {
            currentTargetCrystalsNumber = startTargetCrystalsNumber;
            screenManager.SetTargetNumber(currentTargetCrystalsNumber);

            int randomNumber = Random.Range(0, crystalSprites.sprites.Count);
            targetCrystalColor = (CrystalColor)randomNumber;
            screenManager.SetTargetSprite(randomNumber);
        }

        private void UpdateTargetNumber(CrystalColor crystalColor, int crystalNumber)
        {
            if (crystalColor != targetCrystalColor)
            {
                return;
            }

            currentTargetCrystalsNumber = currentTargetCrystalsNumber - crystalNumber;

            if (currentTargetCrystalsNumber < 1)
            {
                currentTargetCrystalsNumber = 0;
                screenManager.OpenGameOverScreen(currentTargetCrystalsNumber, currentMovesNumber);
            }

            screenManager.SetTargetNumber(currentTargetCrystalsNumber);
        }

        private void RunTime() => Time.timeScale = 1;

        private void PauseTime() => Time.timeScale = 0;

        private void SubscribeToEvents()
        {
            screenManager.openGameScreenEvent += StartGame;
            screenManager.openMenuScreenEvent += PauseTime;
            screenManager.openGameOverScreenEvent += PauseTime;
            screenManager.clickExitButtonEvent += ExitGame;

            crystalController.swapEvent += UpdateMoves;
            crystalController.matchedCrystalsClearedEvent += UpdateTargetNumber;
        }
    }
}
