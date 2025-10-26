using UnityEngine;
using System.Collections.Generic;
using System.Xml.Schema;
using System;

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
    public float minVerticalClearance = 2.0f; // Minimum space above spawn point
    public int maxSpawnAttempts = 100;
    
    [Header("Platform Layer")]
    public LayerMask solidLayers;
    public float spawnCollisionRadius = 0.2f;

    [Header("Boomb Bloom Proximity Settings")]
    // How far awar bomb is from bloom in Twist mode.. so its not impossible
    public float bombBloomMinDistancePercent = 0.4f;
    public float bombBloomMaxDistancePercent = 1.2f;
    public int proximityCheckAttempts = 100; // Attempts per bomb in Twist mode

    private GameObject[] spawnedObjects;
    private List<Vector2> spawnedPositions = new List<Vector2>();

    // Item spawner fields
    private bool isSpawning = false;
    private Transform itemsParent;
    private int lastPlatformIdx = -1;
    private int totalObjects;
    private int runId = 0;
    private int currentItemIdx;

    void Awake()
    {
        itemsParent = new GameObject("SpawnedItems").transform;
        itemsParent.SetParent(transform);
    }

    // private GameObject spawnedObjects;

    void Start()
    {
        totalObjects = bloomCount + bombCount;
        spawnedObjects = new GameObject[totalObjects];
        // SpawnNewLevel();
    }

    public void SpawnNewLevel()
    {
        if (isSpawning) return;
        isSpawning = true;
        runId++;
        Debug.Log($"[ItemSpawner] Spawn run #{runId}");

        currentItemIdx = 0;

        ClearLevel();
        spawnedPositions.Clear();

        List<Rect> platforms = platformSpawner.GetPlatformBounds();
        List<Vector2> bloomPositions = new List<Vector2>();

        if (platformSpawner == null)
        {
            Debug.LogError("PlatformSpawner reference is missing!");
            return;
        }

        if (platforms.Count == 0)
        {
            Debug.LogWarning("No platforms found!");
            return;
        }

        SpawnBlooms(platforms, bloomPositions);
        SpawnBooms(platforms, bloomPositions);
        
        isSpawning = false;
    }

    private void SpawnBlooms(List<Rect> platforms, List<Vector2> bloomPositions)
    {
        int bloomsCreated = 0;
        for (int i = 0; i < bloomCount; i++)
        {
            GameObject obj = SpawnObjectOnPlatform(bloomPrefab, platforms);
            if (obj != null)
            {
                spawnedObjects[currentItemIdx++] = obj;
                bloomPositions.Add(obj.transform.position);
            }
            bloomsCreated++;
        }
        Debug.Log($"SPAWN LEVEL: {bloomsCreated} blooms created");
    }


    private void SpawnBooms(List<Rect> platforms, List<Vector2> bloomPositions)
    {
        int bombsCreated = 0;
        float explosionRadius = levelData.explosionRadius * levelData.radiusMultiplier;


        // This checks if number of bombs > number of blooms so bombs can be paired with blooms nicely so they are spawned closeby
        int guaranteed = Mathf.Min(bombCount, bloomPositions.Count);

        for (int i = 0; i < guaranteed; i++)
        {
            GameObject obj = TrySpawnBombNear(bloomPositions[i], platforms, explosionRadius);
            if (obj == null) obj = SpawnObjectOnPlatform(bombPrefab, platforms);
            if (obj != null)
            {
                spawnedObjects[currentItemIdx++] = obj;
                bombsCreated++;
            }
        }

        for (int i = guaranteed; i < bombCount; i++)
        {
            GameObject obj = null;
            Vector2 bloomPos = bloomPositions[UnityEngine.Random.Range(0, bloomPositions.Count)];
            obj = TrySpawnBombNear(bloomPos, platforms, explosionRadius);
            if (obj == null) obj = SpawnObjectOnPlatform(bombPrefab, platforms);

            if (obj != null)
            {
                spawnedObjects[currentItemIdx++] = obj;
                bombsCreated++;
            }
        }
    

        Debug.Log($"SPAWN LEVEL: {bombsCreated} bombs created");
    }

    
    private GameObject TrySpawnBombNear(Vector2 bloomPos, List<Rect> platforms, float explosionRadius)
    {
        float minDist = Mathf.Min(bombBloomMinDistancePercent, bombBloomMaxDistancePercent) * explosionRadius;
        float maxDist = Mathf.Max(bombBloomMinDistancePercent, bombBloomMaxDistancePercent) * explosionRadius;

        for (int attempt = 0; attempt < proximityCheckAttempts; attempt++)
        {
            float ang = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float dist = UnityEngine.Random.Range(minDist, maxDist);
            Vector2 bombPos = bloomPos + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * dist;

            if (IsTooCloseToOtherItems(bombPos, itemSpacing, bloomPos))continue;
            if (!HasVerticalClearance(bombPos, platforms)) continue;
            if (!IsWithinWorldLimits(bombPos)) continue;
            if (HitsSolid(bombPos)) continue; 

            spawnedPositions.Add(bombPos);
            return Instantiate(bombPrefab, bombPos, Quaternion.identity, itemsParent);
        }
        return null;

    }
    

    private GameObject SpawnObjectOnPlatform(GameObject prefab, List<Rect> platforms)
    {
        // This just makes sure that the items are spawned close enough to the platforms so that they are not impossible to collect
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            int idx = UnityEngine.Random.Range(0, platforms.Count);
            if (platforms.Count > 1 && idx == lastPlatformIdx) 
                idx = (idx + 1) % platforms.Count;
            
            Rect platform = platforms[idx];

            float x = UnityEngine.Random.Range(platform.xMin + 0.3f, platform.xMax - 0.3f);
            float y = platform.yMax + UnityEngine.Random.Range(0.5f, 1.2f);;
            
            Vector2 pos = new Vector2(x, y);

            if (IsTooCloseToOtherItems(pos)) continue;
            if (!HasVerticalClearance(pos, platforms)) continue;
            if (!IsWithinWorldLimits(pos)) continue;
            if (HitsSolid(pos)) continue; 

            spawnedPositions.Add(pos);

            lastPlatformIdx = idx;
            
            return Instantiate(prefab, pos, Quaternion.identity, itemsParent);
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

    private bool IsTooCloseToOtherItems(Vector2 pos, float minSpacing, Vector2? ignorePos = null)
    {
        const float eps = 0.001f;
        foreach (Vector2 existingPos in spawnedPositions)
        {
            if (ignorePos.HasValue && (existingPos - ignorePos.Value).sqrMagnitude < eps)
                continue;
            if (Vector2.Distance(pos, existingPos) < minSpacing)
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
                    return false;
                }
            }
        }
        return true;
    }

    private bool IsWithinWorldLimits(Vector2 pos)
    {
        if (!startLimit || !endLimit) return true; // fail-safe
        float minX = Mathf.Min(startLimit.position.x, endLimit.position.x);
        float maxX = Mathf.Max(startLimit.position.x, endLimit.position.x);
        float minY = Mathf.Min(startLimit.position.y, endLimit.position.y);
        float maxY = Mathf.Max(startLimit.position.y, endLimit.position.y);
        return pos.x >= minX && pos.x <= maxX && pos.y >= minY && pos.y <= maxY;
    }
    
    bool HitsSolid(Vector2 pos)
    {   
        return Physics2D.OverlapCircle(pos, spawnCollisionRadius, solidLayers) != null;
    }

    private void ClearLevel()
    {
        if (itemsParent != null)
        {
            for (int i = itemsParent.childCount - 1; i >= 0; i--)
                Destroy(itemsParent.GetChild(i).gameObject);
        }
        if (spawnedObjects == null) return;
        foreach (var obj in spawnedObjects)
            if (obj != null) Destroy(obj);
    }
}