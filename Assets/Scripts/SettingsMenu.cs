using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider MasterVolumeSlider;
    public Slider MusicVolumeSlider;
    public Slider SXFVolumeSlider;
    public Toggle ProfanityToggle;
    public void SetMasterVolume()
    {
        audioMixer.SetFloat("Master", MasterVolumeSlider.value);
        Debug.Log(MasterVolumeSlider.value);
    }
    public void SetMusicVolume()
    {
        audioMixer.SetFloat("Music", MusicVolumeSlider.value);
        Debug.Log(MusicVolumeSlider.value);
    }
    public void SetSFXVolume()
    {
        audioMixer.SetFloat("SFX", SXFVolumeSlider.value);
        Debug.Log(SXFVolumeSlider.value);
    }
    public void ToggleProfanity()
    {
        if (ProfanityToggle.isOn)
        {
            PlayerPrefs.SetInt("Profanity", 1);
            Debug.Log("Profanity is on");
        }
        else
        {
            PlayerPrefs.SetInt("Profanity", 0);
            Debug.Log("Profanity is off");
        }

    }
}
