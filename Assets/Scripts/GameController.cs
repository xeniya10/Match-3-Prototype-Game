using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private CrystalController crystalController;
    [SerializeField] private TopBar topBar;
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject gameScreen;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TextMeshProUGUI gameResultText;
    private string winResult = "WIN";
    private string loseResult = "Game Over";
    private int moves = 0;
    private int crystalsTarget = 0;

    private void Awake()
    {
        ImageLoader.LoadAll();
        crystalController.CreatePool();
        ShowMenuScreen();
    }

    public void StartGame()
    {
        SetMoves(30);
        SetGameTarget(Random.Range(20, 41));
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
        gameScreen.SetActive(false);
        menuScreen.SetActive(true);
        gameOverScreen.SetActive(false);
    }

    private void ShowGameScreen()
    {
        PauseTime();
        gameScreen.SetActive(true);
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

        int imageNumber = ImageLoader.CrystalSprites.Length;
        Sprite crystal = ImageLoader.CrystalSprites[Random.Range(0, imageNumber)];
        topBar.SetTargetCrystal(crystal);
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

    private void LoadSprites()
    {

    }
}
