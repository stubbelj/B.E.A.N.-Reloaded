using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    Camera cam;
    
    private void Awake()
    {
        cam = Camera.main;
        transform.position = cam.transform.position;
    }

    void Update()
    {
        transform.position = cam.transform.position;
    }
}
