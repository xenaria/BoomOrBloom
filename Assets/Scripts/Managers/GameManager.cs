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

    public IntVariable gameScore;
    public LevelData levelData;

    bool IsNewSession = true;
    private int score = 0;

    public void Awake()
    {
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
    }

    public void GameRestart()
    {
        levelData.Reset();
        // ResetScore();
        // SetScore(score);
        score = 0;
        scoreChange?.Invoke(score);
        gameRestart.Invoke();
        Time.timeScale = 1.0f;
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
        Debug.Log("All blooms collected! You win!");
        Time.timeScale = 0f; // optionally pause game
        gameWin?.Invoke();
        // Add win UI, stop player movement, etc.
    }
    // public void IncreaseScore(int inc)
    // {
    //     SetScore(score + inc);
    //     gameScore.ApplyChange(inc);
    //     Debug.Log($"Added {inc} points to gameScore. \nCurrent gameScore: {gameScore.Value}");
    // }

    // public void SetScore(int newScore)
    // {
    //     score = Mathf.Max(0, newScore);
    //     scoreChange?.Invoke(score);
    // }

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