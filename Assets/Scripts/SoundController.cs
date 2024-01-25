using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour
{
    [SerializeField]
    AudioClip clickSound;
    AudioSource _audio;

    public void PlayClickSound()
    {
        _audio.PlayOneShot(clickSound);
    }

    void ChangeSoundVolume(float val) => _audio.volume = val;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        SettingPanel.AddListenerToSoundVoulumeChanged(ChangeSoundVolume);
    }

    private void OnDestroy() => SettingPanel.RemoveListenerFromSoundVoulumeChanged(ChangeSoundVolume);
}
