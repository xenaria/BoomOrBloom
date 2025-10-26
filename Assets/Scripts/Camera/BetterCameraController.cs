using UnityEngine;

public class BetterCameraController : MonoBehaviour
{
    public Transform player;
    [Tooltip("Bottom-Left bound")] public Transform minBound;
    [Tooltip("Top-Right bound")] public Transform maxBound;

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
        if (!player) player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void LateUpdate()
    {
        if (!player || !cam || !minBound || !maxBound) return;

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        float minX = minBound.position.x + halfW;
        float maxX = maxBound.position.x - halfW;
        float minY = minBound.position.y + halfH;
        float maxY = maxBound.position.y - halfH;

        Vector3 pos = new Vector3(player.position.x, player.position.y, transform.position.z);

        // If the map is smaller than the camera, lock to center on that axis
        if (minX <= maxX) pos.x = Mathf.Clamp(pos.x, minX, maxX);
        else              pos.x = 0.5f * (minBound.position.x + maxBound.position.x);

        if (minY <= maxY) pos.y = Mathf.Clamp(pos.y, minY, maxY);
        else              pos.y = 0.5f * (minBound.position.y + maxBound.position.y);

        transform.position = pos;
    }

    void OnDrawGizmosSelected()
    {
        if (!minBound || !maxBound) return;
        Gizmos.color = Color.yellow;
        Vector3 size = maxBound.position - minBound.position;
        Gizmos.DrawWireCube(minBound.position + size * 0.5f, new Vector3(size.x, size.y, 0f));
    }
}
