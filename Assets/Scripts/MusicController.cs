using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    AudioSource _audio;

    void ChangeMusicVolume(float val) => _audio.volume = val;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        SettingPanel.AddListenerToMusicVoulumeChanged(ChangeMusicVolume);
        _audio.volume = SettingPanel.GetMusicPrefs();
    }

    private void OnDestroy() => SettingPanel.RemoveListenerFromMusicVoulumeChanged(ChangeMusicVolume);
}
