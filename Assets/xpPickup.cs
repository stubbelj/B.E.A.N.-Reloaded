using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class xpPickup : MonoBehaviour
{
    [SerializeField] float waitTime = 1, smoothnessIncreaseMult;
    float value, timeLeft = 0, smoothness = 0.025f;
    PlayerXP player;

    public void Init(PlayerXP player, float value)
    {
        this.player = player;
        this.value = value;
        timeLeft = waitTime;
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft > 0 || !player) return;

        GetComponent<Collider2D>().isTrigger = true;
        GetComponent<Rigidbody2D>().isKinematic = true;
        gameObject.layer = 7;
        smoothness *= smoothnessIncreaseMult;

        transform.position = Vector2.Lerp(transform.position, player.transform.position, Mathf.Min(1, smoothness));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (timeLeft > 0) return;
        var p = collision.GetComponent<PlayerXP>();
        if (!p) return;

        p.AddXP(value);
        Destroy(gameObject);
    }
}
