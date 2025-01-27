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
    public void ToggleProfanity(bool profanity)
    {
        print(profanity);
        Debug.Log(profanity);
    }
}
