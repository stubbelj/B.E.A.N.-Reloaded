using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoHudController : MonoBehaviour
{
    PlayerCombat player => FindObjectOfType<PlayerCombat>();

    [SerializeField] TextMeshProUGUI ammoCount, magCount;
    [SerializeField] Slider ammoSlider;

    private void Update()
    {
        int magLeft = player.GetMagLeft();
        int magCap = player.GetMagCapacity();

        magCount.text = magLeft + "/" + magCap;
        ammoCount.text = player.GetAmmo().ToString();
        ammoSlider.value = (float) magLeft / magCap;

        magCount.color = magLeft == 0 ? Color.red : Color.white;
        ammoCount.color = player.GetAmmo() > 0 ? Color.white : Color.red;
    }
}
