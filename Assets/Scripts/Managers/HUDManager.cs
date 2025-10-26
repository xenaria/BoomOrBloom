using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class HUDManager : MonoBehaviour
{
    private Vector3[] scoreTextPosition = {
        new Vector3(-553f, 438f, 0),
        new Vector3(0,48.036f,0)
    };

    public GameObject scoreText;
    // public GameObject finalScoreText;
    // public GameObject highScoreText;
    public GameObject restartButton;
    public GameObject currentModeText;

    public GameObject gameOverScreen;
    public GameObject gameWinScreen;
    public GameObject pauseMenu;

    public GameManager gameManager;

    private RectTransform scoreTextRect;
    private RectTransform restartButtonRect;
    private int highScore;
    

    void Awake()
    {
        //Debug.Log("HUDManager Awake called");
        scoreTextRect = scoreText.GetComponent<RectTransform>();
        restartButtonRect = restartButton.GetComponent<RectTransform>();

        if (gameManager != null)
        {
            gameManager.gameStart.AddListener(GameStart);
            gameManager.gameOver.AddListener(GameOver);
            gameManager.gameRestart.AddListener(GameStart);
            gameManager.scoreChange.AddListener(SetScore);
            gameManager.gameModeChanged.AddListener(ChangeMode);
            gameManager.gamePause.AddListener(GamePause);
        }
        else
        {
            Debug.LogError("gameManager is NULL!");
        }

    }

    void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.gameStart.RemoveListener(GameStart);
            gameManager.gameOver.RemoveListener(GameOver);
            gameManager.gameRestart.RemoveListener(GameStart);
            gameManager.scoreChange.RemoveListener(SetScore);
            gameManager.gameModeChanged.RemoveListener(ChangeMode);
            gameManager.gamePause.RemoveListener(GamePause);
        }
    }

    public void GameStart()
    {
        Debug.Log("GameStart called!");
        SetScore(gameManager.CurrentScore);

        gameWinScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        
        scoreTextRect.anchoredPosition = scoreTextPosition[0];
    }

    public void SetScore(int score)
    {
        Debug.Log($"SetScore called with: {score}");
        scoreText.GetComponent<TextMeshProUGUI>().text = "Blooms Collected: " + score.ToString() + $"/{gameManager.levelData.totalBlooms.ToString()}";
    }

    public void GameWin()
    {
        gameWinScreen.SetActive(true);
    }
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
    }
    public void ChangeMode(string mode)
    {
        currentModeText.GetComponent<TextMeshProUGUI>().text = "Current mode: " + mode;
    }

    public void GamePause()
    {
        pauseMenu.SetActive(true);
    }
}
