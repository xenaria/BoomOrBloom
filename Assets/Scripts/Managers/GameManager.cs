using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // events
    public UnityEvent gameStart;
    public UnityEvent gameRestart;
    public UnityEvent<int> scoreChange;
    public UnityEvent gameWin;
    public UnityEvent gameOver;
    public UnityEvent gamePause;
    public UnityEvent<string> gameModeChanged;

    public IntVariable gameScore;
    public LevelData levelData;

    bool IsNewSession = true;
    bool isPaused = false;
    private int score = 0;
    
    public static GameManager Instance { get; private set; }

    public void Awake()
    {
        // SINGLETON :(
        if (Instance == null) Instance = this;
        else Destroy(gameObject); 

        if (IsNewSession)
        {
            IsNewSession = false;
            Debug.Log("GAME MANAGER: New session started.");
        }
        else Debug.Log("GAME MANAGER: No new session.");
    }

    void Start()
    {
        gameStart.Invoke();
        Time.timeScale = 1.0f;
        Debug.Log($"Game Mode: {levelData.gameMode}");
    }

    public void ChooseNormalGameMode()
    {
        levelData.gameMode = LevelData.GameMode.Normal;
        gameModeChanged.Invoke("Normal");
        Debug.Log($"Game Mode: {levelData.gameMode}");
    }
    public void ChooseTwistGameMode()
    {
        levelData.gameMode = LevelData.GameMode.Twist;
        gameModeChanged.Invoke("Twist");
        Debug.Log($"Game Mode: {levelData.gameMode}");
    }

    public void GameRestart()
    {
        levelData.Reset();

        score = 0;

        scoreChange?.Invoke(score);
        gameRestart.Invoke();
        gameModeChanged.Invoke(levelData.gameMode.ToString());
        // SoundManager.Instance.RestartMusic();

        Time.timeScale = 1.0f;
    }

    public void GamePause()
    {
        Time.timeScale = 0f;
        isPaused = true;
        // SoundManager.Instance.PauseMusic();
        gamePause.Invoke();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        // SoundManager.Instance.ResumeMusic();
    }

    public void OnBloomCollected()
    {
        levelData.collectedBlooms++;

        score = levelData.collectedBlooms;
        scoreChange?.Invoke(score);

        Debug.Log($"Bloom collected: {levelData.collectedBlooms}/{levelData.totalBlooms}");

        if (levelData.collectedBlooms >= levelData.totalBlooms)
            GameWin();
    }

    private void GameWin()
    {
        levelData.Reset();
        Debug.Log("All blooms collected! You win!");
        gameWin?.Invoke();
        Time.timeScale = 0f;
    }
    
    public void GameOver()
    {
        Time.timeScale = 0.0f;
        gameOver.Invoke();
    }

    public int CurrentScore => score;
    public void ResetScore()
    {
        score = 0; scoreChange?.Invoke(score);
        gameScore.SetValue(0);
        Debug.Log($"Game score reset to {score}. \nHighest score: {gameScore.previousHighestValue.ToString()}");
    }
    
}