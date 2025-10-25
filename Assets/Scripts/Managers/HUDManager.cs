using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class HUDManager : MonoBehaviour
{
    private Vector3[] scoreTextPosition = {
        new Vector3(-601.9744f, 358.8726f, 0),
        new Vector3(0,48.036f,0)
    };
    private Vector3[] restartButtonPosition = {
        new Vector3(801.5745f, 358.8726f, 0),
        new Vector3(0, -98f, 0)
    };

    public GameObject scoreText;
    // public GameObject finalScoreText;
    // public GameObject highScoreText;
    public GameObject restartButton;

    public GameObject gameOverScreen;
    public GameObject gameWinScreen;
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
        }
    }

    public void GameStart()
    {
        Debug.Log("GameStart called!");
        SetScore(gameManager.CurrentScore);

        gameWinScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        
        scoreTextRect.anchoredPosition = scoreTextPosition[0];
        restartButtonRect.anchoredPosition = restartButtonPosition[0];
    }

    public void SetScore(int score)
    {
        Debug.Log($"SetScore called with: {score}");
        scoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + score.ToString();
    }

    public void GameWin()
    {
        gameWinScreen.SetActive(true);
    }
    public void GameOver()
    {
        // finalScoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + gameScore.Value.ToString();
        // highScoreText.GetComponent<TextMeshProUGUI>().text = "High Score: " + gameScore.previousHighestValue.ToString();
        gameOverScreen.SetActive(true);
    }
}
