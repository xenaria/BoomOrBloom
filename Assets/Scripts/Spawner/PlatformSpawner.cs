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
    public float gridSize = 1f;
    [SerializeField] private int maxAttempts = 1000;


    [Header("Min Gaps")]
    public float minHorizontalGap = 1f;
    public float minVerticalGap = 1f;

    private readonly List<Rect> placedRects = new List<Rect>();

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
            size = new Vector2(1.5f, 1.5f);
            prefabToUse = squarePrefab;
        }
        else
        {
            int w = Random.Range(3, 6); 
            size = new Vector2(w, 1f);
            prefabToUse = rectanglePrefab;
        }

        // spawn area rect (min..max from start/end)
        Vector2 min = Vector2.Min((Vector2)startLimit.position, (Vector2)endLimit.position);
        Vector2 max = Vector2.Max((Vector2)startLimit.position, (Vector2)endLimit.position);

        // keep inside bounds accounting for size
        float minX = min.x + size.x * 0.5f;
        float maxX = max.x - size.x * 0.5f;
        float minY = min.y + size.y * 0.5f;
        float maxY = max.y - size.y * 0.5f;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float rx = Random.Range(minX, maxX);
            float ry = Random.Range(minY, maxY);

            // snap to grid
            rx = Mathf.Round(rx / gridSize) * gridSize;
            ry = Mathf.Round(ry / gridSize) * gridSize;

            Vector2 center = new Vector2(rx, ry);
            Rect cand = MakeRect(center, size);

            if (IsValid(cand))
            {
                Instantiate(prefabToUse, new Vector3(center.x, center.y, 0f), Quaternion.identity);
                placedRects.Add(cand);
                return;
            }
        }
        // no valid spot found within attempts; skip spawning this one
    }

    Rect MakeRect(Vector2 center, Vector2 size)
    {
        return new Rect(center - size * 0.5f, size);
    }

    bool IsValid(Rect cand)
    {
        // enforce both gaps: horizontal >= minHorizontalGap AND vertical >= minVerticalGap
        for (int i = 0; i < placedRects.Count; i++)
        {
            Rect r = placedRects[i];

            float gapX = GapAlongAxis(cand.xMin, cand.xMax, r.xMin, r.xMax);
            float gapY = GapAlongAxis(cand.yMin, cand.yMax, r.yMin, r.yMax);

            if (gapX < minHorizontalGap || gapY < minVerticalGap)
                return false;
        }
        return true;
    }

    float GapAlongAxis(float aMin, float aMax, float bMin, float bMax)
    {
        // positive gap if separated, 0 if touching/overlapping
        if (aMax <= bMin) return bMin - aMax;
        if (bMax <= aMin) return aMin - bMax;
        return 0f;
    }
}
