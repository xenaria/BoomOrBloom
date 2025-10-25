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

    [Header("Min Gaps (unused now)")]
    public float minHorizontalGap = 1f;
    public float minVerticalGap = 1f;

    [Header("Packing")]
    public float touchMargin = 0.05f;

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

            Vector2 physSize = size - new Vector2(touchMargin, touchMargin);
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

    // CHANGED: only reject when rectangles overlap (with optional margin)
    bool IsValid(Rect cand)
    {
        for (int i = 0; i < placedRects.Count; i++)
            if (RectsOverlap(cand, placedRects[i], touchMargin))
                return false;
        return true;
    }

    bool RectsOverlap(Rect a, Rect b, float margin)
    {
        return (a.xMin < b.xMax - margin) &&
               (a.xMax > b.xMin + margin) &&
               (a.yMin < b.yMax - margin) &&
               (a.yMax > b.yMin + margin);
    }
}
