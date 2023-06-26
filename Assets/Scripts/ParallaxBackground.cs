using UnityEngine;

public class Parallax : MonoBehaviour
{
    public float parallaxiness;

    Vector2 size;
    float xOffset;
    Transform cam;
    Vector3 oldCamPos;

    private void Start()
    {
        cam = Camera.main.transform;
        oldCamPos = cam.position;
        xOffset = cam.position.x - transform.position.x;
        size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x, GetComponent<SpriteRenderer>().bounds.size.y);
    }

    private void Update()
    {
        var camDelta = cam.position - oldCamPos;
        oldCamPos = cam.position;
        transform.position += camDelta * parallaxiness;

        var xDist = cam.position.x - (transform.position.x + xOffset);
        if (xDist > size.x) transform.position += Vector3.right * size.x;
        if (xDist < -size.x) transform.position += Vector3.left * size.x;
    }
}