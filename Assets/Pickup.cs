using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] int ammoValue, magazineValue;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponentInParent<PlayerCombat>();
        if (!player) return;

        player.AddAmmo(ammoValue, magazineValue);
        Destroy(transform.parent.gameObject);
    }
}
