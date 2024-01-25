using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompletePanel : MonoBehaviour
{
    AudioSource _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        SettingPanel.AddListenerToSoundVoulumeChanged(ChangeSoundVolume);
        _audio.volume = SettingPanel.GetSoundPrefs();
    }

    private void ChangeSoundVolume(float value)
    {
        _audio.volume = value;
    }

    private void OnDestroy()
    {
        SettingPanel.RemoveListenerFromSoundVoulumeChanged(ChangeSoundVolume);
    }
}
