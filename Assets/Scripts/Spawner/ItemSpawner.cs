using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn Area")]
    public float areaSize = 10f;
    public Transform startLimit;
    public Transform endLimit;

    [Header("Booms")]
    public GameObject bombPrefab;
    public int bombCount =8;

    [Header("Blooms")]
    public GameObject cherryPrefab;
    public int cherryCount = 15;

    private GameObject[] spawnedObjects;

    void Start()
    {
        SpawnLevel();
    }

    public void SpawnLevel()
    {
        ClearLevel();

        int totalObjects =+ cherryCount + bombCount;
        spawnedObjects = new GameObject[totalObjects];

        int index = 0;


        // Spawn tokens
        for (int i = 0; i < cherryCount; i++)
            spawnedObjects[index++] = SpawnObject(cherryPrefab);

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
