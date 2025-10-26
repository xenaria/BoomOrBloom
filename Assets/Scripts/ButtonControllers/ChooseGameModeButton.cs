using UnityEngine;

public class ChooseGameModeButton : MonoBehaviour
{
    public LevelData levelData;

    public void chooseNormal()
    {
        levelData.gameMode.Equals("Normal");
        Debug.Log($"GAME MODE CHOSEN: {levelData.gameMode}");
    }
    public void chooseTwist()
    {
        levelData.gameMode.Equals("Twist");
        Debug.Log($"GAME MODE CHOSEN: {levelData.gameMode}");
    }
}
