using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] Sound ambient, music;
    float percent = 0;

    private void Start()
    {
        if (ambient) ambient = Instantiate(ambient);
        music = Instantiate(music);

        if (ambient) ambient.Play();
        music.Play();
        if (ambient) ambient.PercentVolume(0);
        music.PercentVolume(0);
    }

    private void Update()
    {
        percent = Mathf.Lerp(percent, 1, 0.025f);
        if (ambient) ambient.PercentVolume(percent);
        music.PercentVolume(percent);
    }

}
