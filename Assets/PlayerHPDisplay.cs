using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPDisplay : MonoBehaviour
{
    PlayerCombat player => FindObjectOfType<PlayerCombat>();
    [SerializeField] Slider HPSlider;

    private void Update()
    {
        HPSlider.value = player.GetHealthPercent();
    }

}
