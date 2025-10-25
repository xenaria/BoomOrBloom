using UnityEngine;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn Area")]
    public Transform startLimit;
    public Transform endLimit;

    [Header("Booms")]
    public GameObject bombPrefab;
    public int bombCount = 8;

    [Header("Blooms")]
    public GameObject cherryPrefab;
    public int cherryCount = 15;

    [Header("Platform Reference")]
    public PlatformSpawner platformSpawner;

    [Header("Spawn Settings")]
    public float itemSpacing = 0.8f;  // Minimum distance between items
    public int maxSpawnAttempts = 100;

    private GameObject[] spawnedObjects;
    private List<Vector2> spawnedPositions = new List<Vector2>();

    void Start()
    {
        // Wait a frame to ensure platforms are spawned
        Invoke(nameof(SpawnLevel), 0.1f);
    }

    public void SpawnLevel()
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

        int totalObjects = cherryCount + bombCount;
        spawnedObjects = new GameObject[totalObjects];
        int index = 0;

        // Spawn blooms
        for (int i = 0; i < cherryCount; i++)
        {
            GameObject obj = SpawnObjectOnPlatform(cherryPrefab, platforms);
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

    private void ClearLevel()
    {
        if (spawnedObjects == null) return;
        foreach (var obj in spawnedObjects)
            if (obj != null) Destroy(obj);
    }
}