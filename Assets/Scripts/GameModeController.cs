using UnityEngine;
public class GameModeController : MonoBehaviour
{
    public LevelData levelData;
    public GameManager gameManager;
    public void ToggleGameMode()
    {
        if (levelData.gameMode == LevelData.GameMode.Normal)
        {
            levelData.gameMode = LevelData.GameMode.Twist;
            Debug.Log("Game mode switched to TWIST");
        }
        else
        {
            levelData.gameMode = LevelData.GameMode.Normal;
            Debug.Log("Game mode switched to NORMAL");
        }

        gameManager.GameRestart();
    }
}
