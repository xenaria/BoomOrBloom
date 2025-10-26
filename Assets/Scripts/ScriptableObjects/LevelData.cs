using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData")]
public class LevelData : ScriptableObject
{
    public enum GameMode
    {
        Normal,
        Twist
    }

    [Header("Game Mode")]
    public GameMode gameMode = GameMode.Normal;
    
    [Header("Item Counts")]
    public int totalBlooms = 10;
    public int totalBombs = 10;

    [Header("Platform Settings")]
    public int totalPlatforms = 12;
    public float platformGridSize = 1.5f;
    public float minHorizontalGap = 1f;
    public float minVerticalGap = 1f;

    [Header("Bomb Settings")]
    public float fuseTime = 1f;
    public float explosionRadius = 1.2f;
    public float radiusMultiplier = 3f;
    

    [Header("Runtime Tracking")]
    [HideInInspector] public int collectedBlooms = 0;
    [HideInInspector] public int remainingBombs = 0;

    public void Reset()
    {
        collectedBlooms = 0;
        remainingBombs = totalBombs;
    }
}
