using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoHudController : MonoBehaviour
{
    PlayerCombat player => FindObjectOfType<PlayerCombat>();

    [SerializeField] TextMeshProUGUI magText, ammoText;
    [SerializeField] Slider ammoSlider, reloadSlider;
    [SerializeField] GameObject reloadSliderObj, targetBlock;
    private bool reloading;

    private void Update()
    {
        int ammoLeft = player.GetCurAmmo();
        int magCap = player.GetMagCapacity();


        ammoText.text = AddLeadingZeroes(ammoLeft, magCap.ToString().Length) + "/" + magCap;
        magText.text = player.GetMagsLeft().ToString();
        ammoSlider.value = (float) ammoLeft / magCap;

        ammoText.color = ammoLeft == 0 ? Color.red : Color.white;
        magText.color = player.GetMagsLeft() > 0 ? Color.white : Color.red;

        float reloadProg = player.GetReloadProgress();
        if(reloadProg == -1f){
            reloadSliderObj.SetActive(false);
        } else {
            reloadSliderObj.SetActive(true);
            targetBlock.SetActive(player.attempingPerfectReload);
            reloadSlider.value = reloadProg;
        }
    }

    string AddLeadingZeroes(int num, int sigFigs)
    {
        string s = num.ToString();
        while (s.Length < sigFigs) s = "0" + s;
        return s; 
    }
}
