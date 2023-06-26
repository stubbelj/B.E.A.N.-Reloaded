using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Vector2 followSmoothness = new Vector2(0.6f, 0.05f);
    [SerializeField] float forwardOffset = 10, offsetSmoothness = 0.02f;
    float offset;
    [HideInInspector] public Transform player;
    PlayerCombat pCombat;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        pCombat = player.GetComponent<PlayerCombat>();
    }

    private void LateUpdate()
    {
        if (Time.timeScale == 0) return;

        offset = Mathf.Lerp(offset, player.eulerAngles.y == 0 ? forwardOffset : -forwardOffset, offsetSmoothness);
        float x = Mathf.Lerp(transform.position.x, player.position.x + offset, followSmoothness.x);
        float y = Mathf.Lerp(transform.position.y, player.position.y, followSmoothness.y);
        transform.position = new Vector3(x, y, transform.position.z);

        if (pCombat == null || pCombat.isDead) return;
        var mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToViewportPoint(mousePos) * 2 - new Vector3(1, 1, 0);
        float aimPull = pCombat.GetCameraPullDistance();
        var _offset = new Vector3(aimPull * mousePos.x, aimPull * mousePos.y);
        transform.position += (Vector3) _offset;

    }
}
