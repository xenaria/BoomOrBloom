using TMPro;
using UnityEngine;

public class GameModeController : MonoBehaviour
{
    public LevelData levelData;
    public GameManager gameManager;
    public TextMeshProUGUI modeText;

    void Start()
    {
        UpdateModeText();
    }

    public void ToggleGameMode()
    {
        if (levelData.gameMode == LevelData.GameMode.Normal)
        {
            levelData.gameMode = LevelData.GameMode.Twist;
        }
        else
        {
            levelData.gameMode = LevelData.GameMode.Normal;
        }

        UpdateModeText();

        gameManager.gameModeChanged.Invoke(levelData.gameMode.ToString());
        Debug.Log("Game mode switched to " + levelData.gameMode);
    }

    private void UpdateModeText()
    {
        modeText.text = $"{levelData.gameMode}";
    }
}
