using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] float damage, damageTickTime;
    float damageCooldown;

    private void Update()
    {
        damageCooldown -= Time.deltaTime;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>()) {
            if (damageCooldown > 0) return;
            damageCooldown = damageTickTime;
            collision.GetComponent<PlayerCombat>().Hit(damage);
        }
    }
}
