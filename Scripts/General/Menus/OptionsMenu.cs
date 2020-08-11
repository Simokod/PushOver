using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    MusicManager manager;
    [SerializeField]
    Slider BGMSlider;
    [SerializeField]
    Slider SFXSlider;
    private void Start() {
        manager = GameObject.FindGameObjectsWithTag("Music")[0].GetComponent<MusicManager>();
        BGMSlider.value = manager.BGMVal;
        SFXSlider.value = manager.SFXVal;
    }
    public void SetBGMusicVolume(float val) {
        audioMixer.SetFloat("BGMVolume", val);
        manager.BGMVal = val;
    }
    public void SetSFXVolume(float val) {
        manager.SFXVal = val;
        audioMixer.SetFloat("SFXVolume", val);
    }
}
