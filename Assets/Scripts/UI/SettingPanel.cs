using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [SerializeField]
    Slider musicSlider;
    [SerializeField]
    Slider soundSlider;
    static string musicVolumeKey = "musicVolume";
    static string soundVolumeKey = "soundVolume";

    void ChangeMusicSliderParam(float val)
    {
        musicVolume = val;
        if (onMusicVolumeChanged != null)
            onMusicVolumeChanged.Invoke(musicVolume);
        SaveParams();
    }

    void ChangeSoundSliderParam(float val)
    {
        soundVolume = val;
        if (onSoundVolumeChanged != null)
            onSoundVolumeChanged.Invoke(soundVolume);
        SaveParams();
    }

    private void Awake()
    {
        musicSlider.onValueChanged.AddListener(ChangeMusicSliderParam);
        soundSlider.onValueChanged.AddListener(ChangeSoundSliderParam);

        try
        {
            LoadParams();
            musicSlider.value = musicVolume;
            soundSlider.value = soundVolume;
            if (onMusicVolumeChanged != null)
                onMusicVolumeChanged.Invoke(musicVolume);
            if (onSoundVolumeChanged != null)
                onSoundVolumeChanged.Invoke(soundVolume);
        }
        catch 
        {
            musicVolume = musicSlider.value;
            soundVolume = soundSlider.value;
            if(onMusicVolumeChanged != null)
                onMusicVolumeChanged.Invoke(musicVolume);
            if(onSoundVolumeChanged != null)
                onSoundVolumeChanged.Invoke(soundVolume);
            SaveParams();
        }

        gameObject.SetActive(false);
    }

    public delegate void OnSettingsChanged(float value);
    static event OnSettingsChanged onMusicVolumeChanged;
    static event OnSettingsChanged onSoundVolumeChanged;
    public static void AddListenerToMusicVoulumeChanged(OnSettingsChanged method) => onMusicVolumeChanged += method;
    public static void RemoveListenerFromMusicVoulumeChanged(OnSettingsChanged method) => onMusicVolumeChanged -= method;
    public static void AddListenerToSoundVoulumeChanged(OnSettingsChanged method) => onSoundVolumeChanged += method;
    public static void RemoveListenerFromSoundVoulumeChanged(OnSettingsChanged method) => onSoundVolumeChanged -= method;

    static float musicVolume, soundVolume;
    public static float MusicVolume { get { return musicVolume; } }
    public static float SoundVolume { get { return soundVolume; } }
    public static float GetMusicPrefs()
    {
        return PlayerPrefs.GetFloat(musicVolumeKey);
    }
    public static float GetSoundPrefs()
    {
        return PlayerPrefs.GetFloat(soundVolumeKey);
    }


    void SaveParams()
    {
        PlayerPrefs.SetFloat(musicVolumeKey, musicVolume);
        PlayerPrefs.SetFloat(soundVolumeKey, soundVolume);
        Debug.Log("Data Saved");
    }

    void LoadParams()
    {
        if (PlayerPrefs.HasKey(musicVolumeKey))
            musicVolume = PlayerPrefs.GetFloat(musicVolumeKey);
        else
            throw new KeyNotFoundException();

        if (PlayerPrefs.HasKey(soundVolumeKey))
            soundVolume = PlayerPrefs.GetFloat(soundVolumeKey);
        else
            throw new KeyNotFoundException();

        Debug.Log("Data Loaded");
    }
}
