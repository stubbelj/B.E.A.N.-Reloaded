using UnityEngine;

public class Parallax : MonoBehaviour
{
   
    public Camera your_camera;
    public float parallaxiness;
    Vector2 length;
    Vector3 startPos;
    
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;      
        length=GetComponentInChildren<SpriteRenderer>().bounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 relative_pos = your_camera.transform.position * parallaxiness;   
        Vector3 dist=your_camera.transform.position-relative_pos;
        if(dist.x>startPos.x+length.x)
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