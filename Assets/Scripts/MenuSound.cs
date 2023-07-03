using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSound : MonoBehaviour
{
    public Sound uiButton;
    [SerializeField] Sound impact, hoverSound;

    private void OnEnable()
    {
        if (uiButton) uiButton = Instantiate(uiButton);
        if (impact) impact = Instantiate(impact);
        if (hoverSound) hoverSound = Instantiate(hoverSound);
    }

    public void ButtonSound() {
        if (!uiButton.instantialized) uiButton = Instantiate(uiButton);
        uiButton.Play();
    }

    public void PlayHover()
    {
        hoverSound.Play();
    }

    public void PlayImpact()
    {
        impact.Play();
    }
}
