using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public Sound SMGFire, jump, weakPunch, strongPunch, punchHit, ammoPickup, magRefillPickup, slamLand, footStep, punch3WindUp, landingSound, 
        dash, levelUp, playerHurt, reload, perfectReload, playerDeath, heartBeat, bulletReady;
    [SerializeField] float maxFallSpeed = 30;
    Rigidbody2D rb => GetComponent<Rigidbody2D>();

    private void Start()
    {
        magRefillPickup = Instantiate(magRefillPickup);
        punch3WindUp = Instantiate(punch3WindUp);
        landingSound = Instantiate(landingSound);
        strongPunch = Instantiate(strongPunch);
        ammoPickup = Instantiate(ammoPickup);
        weakPunch = Instantiate(weakPunch);
        punchHit = Instantiate(punchHit);
        slamLand = Instantiate(slamLand);
        footStep = Instantiate(footStep);
        SMGFire = Instantiate(SMGFire);
        jump = Instantiate(jump);
        dash = Instantiate(dash);
        levelUp = Instantiate(levelUp);
        playerHurt = Instantiate(playerHurt);
        reload = Instantiate(reload);
        perfectReload = Instantiate(perfectReload);
        playerDeath = Instantiate(playerDeath);
        bulletReady = Instantiate(bulletReady);

        heartBeat = Instantiate(heartBeat);
        heartBeat.PlaySilent();
    }

    public void Land()
    {
        if (rb.velocity.y != 0 && rb.velocity.y < maxFallSpeed / 2) return;

        landingSound.Play();
        if (rb.velocity.y != 0) landingSound.PercentVolume(Mathf.Abs(rb.velocity.y) / maxFallSpeed);
        float cameraShake = 0.2f;
        cameraShake *= rb.velocity.y == 0 ? 1 : Mathf.Abs(rb.velocity.y) / maxFallSpeed;
        CameraShake.i.Shake(cameraShake, 0.2f);
    }
}
