using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSound : MonoBehaviour
{
    public Sound uiButton;

    private void Start()
    {
        uiButton = Instantiate(uiButton);
    }

    public void ButtonSound() {
        uiButton.Play();
    }
}
