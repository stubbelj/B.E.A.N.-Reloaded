using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeScript : MonoBehaviour
{
    [SerializeField] private AudioMixer MusicMixer, SFXMixer;
    public float masterVolume, baseMusicVolume, baseSfxVolume = 1f;
    private float finalMusicVolume, finalSfxVolume = 1f;

    public void setMasterVolume(float value) 
    {
        masterVolume = value;
        finalMusicVolume = baseMusicVolume * masterVolume;
        finalSfxVolume = baseSfxVolume * masterVolume;

        MusicMixer.SetFloat("Volume", Mathf.Log10(finalMusicVolume) * 20);
        SFXMixer.SetFloat("Volume", Mathf.Log10(finalSfxVolume) * 20);
    }

    public void setMusicVolume(float value){
        baseMusicVolume = value;
        finalMusicVolume = baseMusicVolume * masterVolume;

        MusicMixer.SetFloat("Volume", Mathf.Log10(finalMusicVolume) * 20);
    }

    public void setSFXVolume(float value){
        baseSfxVolume = value;
        finalSfxVolume = baseSfxVolume * masterVolume;

        SFXMixer.SetFloat("Volume", Mathf.Log10(finalSfxVolume) * 20);
    }
}
