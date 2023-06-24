using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class xpPickup : MonoBehaviour
{
    [SerializeField] float waitTime = 1;
    float value;
    float timeLeft = 0;
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
        if (timeLeft > 0) return;

        transform.position = Vector2.Lerp(transform.position, player.transform.position, 0.025f);
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
