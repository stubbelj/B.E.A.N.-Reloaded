using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] GameObject playerMarkerPrefab;
    GameObject playerMarker;
    Transform player => FindObjectOfType<PlayerController>().transform;

    private void Start()
    {
        playerMarker = Instantiate(playerMarkerPrefab, player.transform.position + Vector3.back * 11, Quaternion.identity, transform);
    }

    private void Update()
    {
        var pos = player.position;
        pos.z = -11;
        playerMarker.transform.position = pos;
    }
}
