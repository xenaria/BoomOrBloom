using UnityEngine;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    public LevelData levelData;
    public PlatformSpawner platformSpawner;
    int bloomCount => levelData.totalBlooms;
    int bombCount => levelData.totalBombs;

    [Header("Spawn Area")]
    public Transform startLimit;
    public Transform endLimit;

    [Header("Booms")]
    public GameObject bombPrefab;

    [Header("Blooms")]
    public GameObject bloomPrefab;

    [Header("Spawn Settings")]
    public float itemSpacing = 1f;  // Minimum distance between items
    public float minVerticalClearance = 1.5f; // Minimum space above spawn point
    public int maxSpawnAttempts = 100;

    private GameObject[] spawnedObjects;
    private List<Vector2> spawnedPositions = new List<Vector2>();

    void Start()
    {
        
    }

    public void SpawnNewLevel()
    {
        ClearLevel();
        spawnedPositions.Clear();

        if (platformSpawner == null)
        {
            Debug.LogError("PlatformSpawner reference is missing!");
            return;
        }

        List<Rect> platforms = platformSpawner.GetPlatformBounds();
        if (platforms.Count == 0)
        {
            Debug.LogWarning("No platforms found!");
            return;
        }

        int totalObjects = bloomCount + bombCount;
        spawnedObjects = new GameObject[totalObjects];
        int index = 0;

        // Spawn blooms
        for (int i = 0; i < bloomCount; i++)
        {
            GameObject obj = SpawnObjectOnPlatform(bloomPrefab, platforms);
            if (obj != null) spawnedObjects[index++] = obj;
        }

        // Spawn booms
        for (int i = 0; i < bombCount; i++)
        {
            GameObject obj = SpawnObjectOnPlatform(bombPrefab, platforms);
            if (obj != null) spawnedObjects[index++] = obj;
        }
    }

    private GameObject SpawnObjectOnPlatform(GameObject prefab, List<Rect> platforms)
    {
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            // Pick random platform
            Rect platform = platforms[Random.Range(0, platforms.Count)];

            // Spawn on top of platform with small offset
            float x = Random.Range(platform.xMin + 0.3f, platform.xMax - 0.3f);
            float y = platform.yMax + 0.5f;
            
            Vector2 pos = new Vector2(x, y);

            // Check if position is far enough from other items
            if (IsTooCloseToOtherItems(pos))
                continue;

            // Check if there's enough vertical clearance above spawn point
            if (!HasVerticalClearance(pos, platforms))
                continue;

            spawnedPositions.Add(pos);
            return Instantiate(prefab, pos, Quaternion.identity);
        }

        Debug.LogWarning($"Failed to spawn {prefab.name} after {maxSpawnAttempts} attempts");
        return null;
    }

    private bool IsTooCloseToOtherItems(Vector2 pos)
    {
        foreach (Vector2 existingPos in spawnedPositions)
        {
            if (Vector2.Distance(pos, existingPos) < itemSpacing)
                return true;
        }
        return false;
    }

    private bool HasVerticalClearance(Vector2 spawnPos, List<Rect> platforms)
    {
        // Check if any platform blocks the space above the spawn point
        foreach (Rect platform in platforms)
        {
            // Check if this platform is above our spawn position
            if (platform.yMin > spawnPos.y && platform.yMin < spawnPos.y + minVerticalClearance)
            {
                // Check if the platform horizontally overlaps with our spawn position
                if (spawnPos.x >= platform.xMin && spawnPos.x <= platform.xMax)
                {
                    return false; // Not enough clearance - platform is blocking
                }
            }
        }
        return true; // Enough clearance - no platforms blocking above
    }

    private void ClearLevel()
    {
        if (spawnedObjects == null) return;
        foreach (var obj in spawnedObjects)
            if (obj != null) Destroy(obj);
    }
}