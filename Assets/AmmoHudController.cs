using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoHudController : MonoBehaviour
{
    PlayerCombat player => FindObjectOfType<PlayerCombat>();

    [SerializeField] TextMeshProUGUI ammoCount;
    [SerializeField] Slider ammoSlider;

    private void Update()
    {
        int magLeft = player.GetMagLeft();
        int magCap = player.GetMagCapacity();

        ammoCount.text = magLeft + "/" + magCap;
        ammoSlider.value = (float) magLeft / magCap;

        ammoCount.color = magLeft == 0 ? Color.red : Color.white;
    }
}
