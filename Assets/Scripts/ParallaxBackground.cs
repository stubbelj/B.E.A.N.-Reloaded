using UnityEngine;

public class Parallax : MonoBehaviour
{

    public float parallaxiness;

    Vector2 size;
    Vector3 startPos;
    Transform cam;
    Vector3 oldCamPos;

    private void Start()
    {
        cam = Camera.main.transform;
        oldCamPos = cam.position;
    }

    private void Update()
    {
        var camDelta = cam.position - oldCamPos;
        oldCamPos = cam.position;

        transform.position += camDelta * parallaxiness; 
    }

    /*public float parallaxiness;

    Vector2 size;
    Vector3 startPos;
    GameObject cam;
    Vector3 oldCamPos;

    void Start() {
        cam = transform.parent.gameObject;
        startPos = transform.position;
        size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x, GetComponent<SpriteRenderer>().bounds.size.y);
    }

    void Update() {

        Vector2 delta = new Vector2(cam.transform.position.x * parallaxiness, cam.transform.position.y * parallaxiness);
        transform.position = new Vector3(startPos.x + delta.x, startPos.y + delta.y, 0);

        float totalDelta = cam.transform.position.x * (1 - parallaxiness);
        if (totalDelta > startPos.x + size.x) {
            startPos += new Vector3(size.x, 0, 0);
        } else if (totalDelta < startPos.x - size.x) {
            startPos -= new Vector3(size.x, 0, 0);
        }
    }*/
}