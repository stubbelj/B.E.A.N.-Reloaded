using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public Sound SMGFire, jump, weakPunch, strongPunch, punchHit, ammoPickup, magRefillPickup, slamLand, footStep, punch3WindUp;

    private void Start()
    {
        SMGFire = Instantiate(SMGFire);
        jump = Instantiate(jump);
        weakPunch = Instantiate(weakPunch);
        strongPunch = Instantiate(strongPunch);
        punchHit = Instantiate(punchHit);
        ammoPickup = Instantiate(ammoPickup);
        magRefillPickup = Instantiate(magRefillPickup);
        slamLand = Instantiate(slamLand);
        footStep = Instantiate(footStep);
        punch3WindUp = Instantiate(punch3WindUp);
    }
}
