using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("Item Counts")]
    public int totalBlooms = 10;
    public int totalBombs = 10;

    [Header("Platform Settings")]
    public int totalPlatforms = 12;
    public float platformGridSize = 1.5f;
    public float minHorizontalGap = 1f;
    public float minVerticalGap = 1f; 
    

    [Header("Runtime Tracking")]
    [HideInInspector] public int collectedBlooms = 0;
    [HideInInspector] public int remainingBombs = 0;

    public void Reset()
    {
        collectedBlooms = 0;
        remainingBombs = totalBombs;
    }
}
