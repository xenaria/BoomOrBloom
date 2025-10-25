using UnityEngine;
using System.Collections.Generic;

public class PlatformSpawner : MonoBehaviour
{
    [Header("Platform Prefabs")]
    public GameObject rectanglePrefab;
    public GameObject squarePrefab;

    [Header("Spawn Settings")]
    public int numberOfPlatforms = 20;
    public Transform startLimit;
    public Transform endLimit;
    public float gridSize = 0.1f;  
    [SerializeField] private int maxAttempts = 5000;

    [Header("Platform Spacing")]
    public float minHorizontalGap = 2f;
    public float minVerticalGap = 2f; 

    private readonly List<Rect> placedRects = new List<Rect>();
    public LayerMask platformLayer;

    void Start()
    {
        for (int i = 0; i < numberOfPlatforms; i++)
            TrySpawnOne();
    }

    void TrySpawnOne()
    {
        bool isSquare = Random.value < 0.5f;
        Vector2 size;
        GameObject prefabToUse;

        if (isSquare)
        {
            prefabToUse = squarePrefab;
            size = new Vector2(1.5f, 1.5f); 
        }
        else
        {
            prefabToUse = rectanglePrefab;
            size = new Vector2(6f, 1.5f);  
        }

        Vector2 min = Vector2.Min((Vector2)startLimit.position, (Vector2)endLimit.position);
        Vector2 max = Vector2.Max((Vector2)startLimit.position, (Vector2)endLimit.position);

        float minX = min.x + size.x * 0.5f;
        float maxX = max.x - size.x * 0.5f;
        float minY = min.y + size.y * 0.5f;
        float maxY = max.y - size.y * 0.5f;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float rx = Mathf.Round(Random.Range(minX, maxX) / gridSize) * gridSize;
            float ry = Mathf.Round(Random.Range(minY, maxY) / gridSize) * gridSize;

            Vector2 center = new Vector2(rx, ry);
            Rect cand = MakeRect(center, size);

            if (!IsValid(cand)) continue;

            Vector2 physSize = size - new Vector2(0.05f, 0.05f);
            if (Physics2D.OverlapBox(center, physSize, 0f, platformLayer) != null) continue;
            
            Instantiate(prefabToUse, new Vector3(center.x, center.y, 0f), Quaternion.identity);
            placedRects.Add(cand);
            return;
        }
    }

    Rect MakeRect(Vector2 center, Vector2 size)
    {
        return new Rect(center - size * 0.5f, size);
    }

    
    bool IsValid(Rect cand)
    {
        for (int i = 0; i < placedRects.Count; i++)
        {
            Rect placed = placedRects[i];
            
            // Check horizontal gap
            float horizontalGap = Mathf.Max(
                cand.xMin - placed.xMax,
                placed.xMin - cand.xMax
            );
            
            // Check vertical gap
            float verticalGap = Mathf.Max(
                cand.yMin - placed.yMax,
                placed.yMin - cand.yMax
            );
            
            // If overlapping or gap is too small, reject
            if (horizontalGap < minHorizontalGap && verticalGap < minVerticalGap)
                return false;
        }
        return true;
    }
    
    public List<Rect> GetPlatformBounds()
    {
        return new List<Rect>(placedRects);
    }
}