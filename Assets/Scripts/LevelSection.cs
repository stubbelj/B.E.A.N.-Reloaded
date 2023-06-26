using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LevelSection : MonoBehaviour
{
    public Transform startPoint, endPoint;
    public Transform player;
    public LevelGenerator levelGen;
    bool spawnedNext;

    private void Update()
    {
        startPoint.transform.position = transform.position;

        if (!Application.isPlaying) return;

        if (!spawnedNext && player.position.x > endPoint.position.x)
        {
            spawnedNext = true;
            levelGen.PlaceNextSection();
        }
    }
}
