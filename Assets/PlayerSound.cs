using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public Sound SMGFire, jump, weakPunch, strongPunch;

    private void Start()
    {
        SMGFire = Instantiate(SMGFire);
        jump = Instantiate(jump);
        weakPunch = Instantiate(weakPunch);
        strongPunch = Instantiate(strongPunch);
    }


}
