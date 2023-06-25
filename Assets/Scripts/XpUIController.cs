using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XpUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Slider xpSlider;

    PlayerXP pXP => FindObjectOfType<PlayerXP>();

    private void Update()
    {
        levelText.text = "Level" + pXP.getLevel();
        xpSlider.value = pXP.GetXPPercent();
    }
}
