using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.SoundMemory
{
    public class SoundManager_AudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private List<AudioClip> soundClips = new List<AudioClip>();

        [Header("Settings")]
        [SerializeField] private float _soundDelay = 0.5f;
        public float SoundDelay => _soundDelay;

        public void PlaySound(int index)
        {
            if (index >= 0 && index < soundClips.Count && soundClips[index] != null)
            {
                _audioSource.PlayOneShot(soundClips[index]);
            }
        }

        public float GetSoundLength(int index)
        {
            if (index >= 0 && index < soundClips.Count && soundClips[index] != null)
            {
                return soundClips[index].length;
            }
            return 0f;
        }
    }
}
