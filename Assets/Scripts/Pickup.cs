using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour 
{
    public enum PickupType { AMMO, GUN }

    [SerializeField] float waitTime = 1;
    
    public PickupType type;
    [ConditionalEnumHide(nameof(type), (int)PickupType.AMMO)]
    [SerializeField] int ammoValue, magazineValue;
    [ConditionalEnumHide(nameof(type), (int)PickupType.GUN)]
    [SerializeField] Gun gun;

    public void Init(Gun gun, float waitTime)
    {
        this.gun = gun;
        this.waitTime = waitTime;
    }

    private void Update()
    {
        waitTime -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponentInParent<PlayerCombat>();
        if (!player || waitTime > 0) return;

        if (type == PickupType.AMMO) player.AddAmmo(ammoValue, magazineValue);
        else if (type == PickupType.GUN) player.GetNewGun(gun);

        Destroy(transform.parent.gameObject);
    }
}
