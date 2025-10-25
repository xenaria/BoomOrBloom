using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn Area")]
    public float areaSize = 10f;

    [Header("Platforms")]
    public GameObject platformPrefab;
    public int platformCount = 20;

    [Header("Tokens")]
    public GameObject tokenPrefab;
    public int tokenCount = 5;

    [Header("Bombs")]
    public GameObject bombPrefab;
    public int bombCount = 3;

    private GameObject[] spawnedObjects;

    public void SpawnLevel()
    {
        ClearLevel();

        int totalObjects = platformCount + tokenCount + bombCount;
        spawnedObjects = new GameObject[totalObjects];

        int index = 0;

        // Spawn platforms
        for (int i = 0; i < platformCount; i++)
            spawnedObjects[index++] = SpawnObject(platformPrefab);

        // Spawn tokens
        for (int i = 0; i < tokenCount; i++)
            spawnedObjects[index++] = SpawnObject(tokenPrefab);

        // Spawn bombs
        for (int i = 0; i < bombCount; i++)
            spawnedObjects[index++] = SpawnObject(bombPrefab);
    }

    private GameObject SpawnObject(GameObject prefab)
    {
        float x = Random.Range(-areaSize / 2f, areaSize / 2f);
        float y = Random.Range(-areaSize / 2f, areaSize / 2f);
        Vector2 pos = new Vector2(x, y);

        return Instantiate(prefab, pos, Quaternion.identity);
    }

    private void ClearLevel()
    {
        if (spawnedObjects != null)
        {
            foreach (var obj in spawnedObjects)
                if (obj != null) Destroy(obj);
        }
    }
}
