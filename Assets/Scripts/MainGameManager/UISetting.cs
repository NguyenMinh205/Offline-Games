using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISetting : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private float musicVolume;
    private float sfxVolume;
    private AudioManager audioManager;

    private void OnEnable()
    {
        audioManager = AudioManager.Instance;
        if (audioManager == null)
        {
            Debug.LogError("AudioManager instance not found!");
            return;
        }

        musicVolume = DataManager.Instance.GameData.MusicVolume;
        sfxVolume = DataManager.Instance.GameData.SoundVolume;

        if (musicSlider != null)
        {
            musicSlider.value = musicVolume;
            musicSlider.onValueChanged.AddListener(UpdateMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVolume;
            sfxSlider.onValueChanged.AddListener(UpdateSfxVolume);
        }
    }

    public void UpdateMusicVolume(float value)
    {
        if (audioManager != null && audioManager.MusicSource != null)
        {
            audioManager.SetMusicVolume(value);
        }
    }

    public void UpdateSfxVolume(float value)
    {
        if (audioManager != null && audioManager.SoundSource != null)
        {
            audioManager.SetSoundVolume(value);
        }
    }
}
