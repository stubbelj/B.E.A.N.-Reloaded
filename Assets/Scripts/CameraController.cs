using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Vector2 followSmoothness = new Vector2(0.6f, 0.05f);
    [SerializeField] float downFollowSmoothness = 0.5f, forwardOffset = 10, offsetSmoothness = 0.02f;
    float offset;
    [HideInInspector] public Transform player;
    PlayerCombat pCombat;

    [Header("lookAhead")]
    [SerializeField] Vector2 maxPlayerSpeed, maxLookAhead;
    [SerializeField] float lookAheadSmoothness;
    public bool lookAheadUp;
    Rigidbody2D rb;
    Vector2 speedOffset;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        rb = player.GetComponent<Rigidbody2D>();
        pCombat = player.GetComponent<PlayerCombat>();
    }

    private void LateUpdate()
    {
        if (Time.timeScale == 0) return;

        Vector3 playerPos = player.position;
        Vector2 playerSpeed = rb.velocity;
        float percentMaxX = playerSpeed.x < 0 ? playerSpeed.x/-maxPlayerSpeed.x : playerSpeed.x/maxPlayerSpeed.x;
        float percentMaxY = playerSpeed.x < 0 ? playerSpeed.y/-maxPlayerSpeed.y : playerSpeed.y/maxPlayerSpeed.y;
        playerSpeed.x = Mathf.Min(1, Mathf.Max(-1, percentMaxX));
        playerSpeed.y = Mathf.Min(1, Mathf.Max(-1, percentMaxY));
        if (!lookAheadUp && playerSpeed.y > 0) playerSpeed.y = 0;

        Vector2 _speedOffset = new Vector2(playerSpeed.x * maxLookAhead.x, playerSpeed.y * maxLookAhead.y);
        //speedOffset = _speedOffset.y < 0 ? new Vector2(speedOffset.x, _speedOffset.y) : speedOffset;
        speedOffset = Vector2.Lerp(speedOffset, _speedOffset, lookAheadSmoothness);
        playerPos += (Vector3) speedOffset;

        //print("speedOffset: " + speedOffset + ", _speedOffset: " + _speedOffset + ", playerSpeed: " + playerSpeed);

        offset = Mathf.Lerp(offset, player.eulerAngles.y == 0 ? forwardOffset : -forwardOffset, offsetSmoothness);
        float x = Mathf.Lerp(transform.position.x, playerPos.x + offset, followSmoothness.x);
        float y = Mathf.Lerp(transform.position.y, playerPos.y, playerSpeed.y < 0 ? downFollowSmoothness : followSmoothness.y);
        transform.position = new Vector3(x, y, transform.position.z);

        if (pCombat == null || pCombat.isDead) return;

        var mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToViewportPoint(mousePos) * 2 - new Vector3(1, 1, 0);
        float aimPull = pCombat.GetCameraPullDistance();
        var _offset = new Vector3(aimPull * mousePos.x, aimPull * mousePos.y);
        transform.position += (Vector3) _offset;

    }
}
