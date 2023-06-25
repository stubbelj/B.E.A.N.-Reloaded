using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Vector2 followSmoothness = new Vector2(0.6f, 0.05f);
    [SerializeField] float forwardOffset = 10, offsetSmoothness = 0.02f;
    float offset;
    public Transform player;

    public void Start(){
        player = FindObjectOfType<PlayerController>().transform;
    }
    private void LateUpdate()
    {
        offset = Mathf.Lerp(offset, player.eulerAngles.y == 0 ? forwardOffset : -forwardOffset, offsetSmoothness);
        float x = Mathf.Lerp(transform.position.x, player.position.x + offset, followSmoothness.x);
        float y = Mathf.Lerp(transform.position.y, player.position.y, followSmoothness.y);
        transform.position = new Vector3(x, y, transform.position.z);
    }
}
