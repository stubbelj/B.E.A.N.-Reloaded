using UnityEngine;

public class Parallax : MonoBehaviour
{
   
    public float parallaxiness;
    Camera cam;
    Vector2 length;
    Vector3 startPos;
    
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        startPos = transform.position;      
        length = GetComponentInChildren<SpriteRenderer>().bounds.size;
        //print
        transform.position = GameObject.Find("Player").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 relative_pos = cam.transform.position * parallaxiness;   
        Vector3 dist = cam.transform.position-relative_pos;
        if(dist.x > startPos.x + length.x)
        {
            startPos.x += length.x;
        }
        if(dist.x < startPos.x - length.x)
        {
            startPos.x -= length.x;
        }  
        relative_pos.z = startPos.z;

        transform.position = startPos - relative_pos;      
    }
}