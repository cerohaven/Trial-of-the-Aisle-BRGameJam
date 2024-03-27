using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuAudioSliders : MonoBehaviour
{
    //Store all audio source volumes

    //Get the current volume of the audioSource
    [Header("Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    [Header("Slider Texts")]
    [SerializeField] private TextMeshProUGUI masterValue;
    [SerializeField] private TextMeshProUGUI sfxValue;
    [SerializeField] private TextMeshProUGUI musicValue;

    private void Start()
    {
        LoadVolume();
    }

    //called from inspector on the slider
    public void Slider_MasterVolume(float _volume)
    {
        float scaledVolume = _volume * 100; //now it's between 0-100 instead of 0-1
        masterValue.text = scaledVolume.ToString("00");
        AudioManager.Instance.m_AudioMixer.SetFloat("Master", Mathf.Log10(_volume) * 20);

        //Saving the value
        PlayerPrefs.SetFloat("MasterVolume", _volume);
    }

    public void Slider_SFXVolume(float _volume)
    {
        float scaledVolume = _volume * 100; //now it's between 0-100 instead of 0-1
        sfxValue.text = scaledVolume.ToString("00");
        AudioManager.Instance.m_AudioMixer.SetFloat("SFX", Mathf.Log10(_volume) * 20);

        //Saving the value
        PlayerPrefs.SetFloat("SFXVolume", _volume);
    }

    public void Slider_MusicVolume(float _volume)
    {
        float scaledVolume = _volume * 100; //now it's between 0-100 instead of 0-1
        musicValue.text = scaledVolume.ToString("00");
        AudioManager.Instance.m_AudioMixer.SetFloat("Music", Mathf.Log10(_volume) * 20);

        //Saving the value
        PlayerPrefs.SetFloat("MusicVolume", _volume);
    }

    private void LoadVolume()
    {
        float masterSliderValue = PlayerPrefs.GetFloat("MasterVolume");
        float sfxSliderValue = PlayerPrefs.GetFloat("SFXVolume");
        float musicSliderValue = PlayerPrefs.GetFloat("MusicVolume");
       
        
        masterVolumeSlider.value = masterSliderValue;
        sfxSlider.value = sfxSliderValue;
        musicSlider.value = musicSliderValue;

        masterValue.text = (masterSliderValue * 100).ToString("00");
        musicValue.text = (sfxSliderValue * 100).ToString("00");
        sfxValue.text = (musicSliderValue * 100).ToString("00");

    }

   
}
