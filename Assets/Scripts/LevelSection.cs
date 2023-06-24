using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LevelSection : MonoBehaviour
{
    public Transform startPoint, endPoint;

    private void Update()
    {
        startPoint.transform.position = transform.position;
    }
}
