using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private CrystalController crystalController;
    [SerializeField] private ScreenManager screenManager;
    [SerializeField] private CrystalSprites crystalSprites;
    private int currentMoves = 0;
    private int currentTarget = 0;
    private int TargetCrystalType = 0;

    [Header("Game start options")]
    public int StartMoves = 30;
    public int StartTarget = 20;

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
        crystalController.FindMatch();
    }

    private void CheckGameOver()
    {
        if (currentTarget < 1 || currentMoves < 1)
        {
            screenManager.OpenGameOver(currentTarget, currentMoves);
        }
    }

    public void ExitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    private void ResetMoves()
    {
        currentMoves = StartMoves;
        screenManager.SetMovesNumber(currentMoves);
    }

    private void UpdateMoves()
    {
        currentMoves = currentMoves - 1;
        screenManager.SetMovesNumber(currentMoves);

        if (currentMoves == 0)
        {
            screenManager.OpenGameOver(currentTarget, currentMoves);
        }
    }

    private void ResetGameTarget()
    {
        currentTarget = StartTarget;
        screenManager.SetTargetNumber(currentTarget);

        TargetCrystalType = Random.Range(0, crystalSprites.Sprites.Count);
        screenManager.SetTargetSprite(TargetCrystalType);
    }

    private void UpdateTargetNumber(int crystalType, int crystalNumber)
    {
        if (crystalType == TargetCrystalType)
        {
            currentTarget = currentTarget - crystalNumber;

            if (currentTarget < 1)
            {
                currentTarget = 0;
                screenManager.OpenGameOver(currentTarget, currentMoves);
            }

            screenManager.SetTargetNumber(currentTarget);
        }
    }

    private void RunTime() => Time.timeScale = 1;

    private void PauseTime() => Time.timeScale = 0;

    private void SubscribeToEvents()
    {
        screenManager.OpenGameScreenEvent += StartGame;
        screenManager.OpenMenuScreenEvent += PauseTime;
        screenManager.OpenGameOverScreenEvent += PauseTime;
        screenManager.ClickExitButtonEvent += ExitGame;

        crystalController.MoveMadeEvent += UpdateMoves;
        crystalController.ClearMatchEvent += (crystalType, crystalNumber) => UpdateTargetNumber(crystalType, crystalNumber);
    }
}
