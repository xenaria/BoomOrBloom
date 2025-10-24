using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Transform startLimit;
    public Transform endLimit;

    private float offsetX, offsetY;
    private float minX, maxX, minY, maxY;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        if (!player) player = GameObject.FindGameObjectWithTag("Player").transform;

        offsetX = transform.position.x - player.position.x;
        offsetY = transform.position.y - player.position.y;

        float halfWidth = cam.orthographicSize * cam.aspect;
        float halfHeight = cam.orthographicSize;

        // Horizontal limits
        if (startLimit)
            minX = startLimit.position.x + halfWidth;
        else
            minX = float.NegativeInfinity;

        if (endLimit)
            maxX = endLimit.position.x - halfWidth;
        else
            maxX = float.PositiveInfinity;

        // Vertical limits
        if (startLimit)
            minY = startLimit.position.y + halfHeight;
        else
            minY = float.NegativeInfinity;

        if (endLimit)
            maxY = endLimit.position.y - halfHeight;
        else
            maxY = float.PositiveInfinity;
    }

    void LateUpdate()
    {
        if (!player) return;

        float targetX = player.position.x + offsetX;
        float targetY = player.position.y + offsetY;

        // Clamp both axes
        targetX = Mathf.Clamp(targetX, minX, maxX);
        targetY = Mathf.Clamp(targetY, minY, maxY);

        transform.position = new Vector3(targetX, targetY, transform.position.z);
    }
}
