using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] Sound ambient, music;
    float percent = 0;

    private void Start()
    {
        ambient = Instantiate(ambient);
        music = Instantiate(music);

        ambient.Play();
        music.Play();
        ambient.PercentVolume(0);
        music.PercentVolume(1);
    }

    private void Update()
    {
        percent = Mathf.Lerp(percent, 1, 0.025f);
        ambient.PercentVolume(percent);
        music.PercentVolume(percent);
    }

}
