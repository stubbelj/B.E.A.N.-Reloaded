using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPDisplay : MonoBehaviour
{
    PlayerCombat player;
    [SerializeField] Slider HPSlider;
    [SerializeField] Image redOverlay;
    [SerializeField] GameObject xpParent;
    float maxRed;
    private void Start()
    {
        player = FindObjectOfType<PlayerCombat>();
        maxRed = redOverlay.color.a;
        redOverlay.gameObject.SetActive(true);

        if (!FindObjectOfType<LevelGenerator>()) xpParent.SetActive(false);
    }

    private void Update()
    {
        HPSlider.value = player.GetHealthPercent();

        Color red = redOverlay.color;
        red.a = maxRed * (1 - HPSlider.value);
        redOverlay.color = red;
    }

}
