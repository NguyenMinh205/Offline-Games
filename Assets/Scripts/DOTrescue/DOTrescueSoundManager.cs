using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.DOTrescue
{
    public class DOTrescueSoundManager : Singleton<DOTrescueSoundManager>
    {
        [SerializeField] private AudioSource _soundSource;

        [SerializeField] private AudioClip _clickSound;
        [SerializeField] private AudioClip _moveSound;
        [SerializeField] private AudioClip _pointSound;
        [SerializeField] private AudioClip _loseSound;

        public void PlayClickSound()
        {
            _soundSource.PlayOneShot(_clickSound);
        }

        public void PlayMoveSound()
        {
            _soundSource.PlayOneShot(_moveSound);
        }    

        public void PlayPointSound()
        {
            _soundSource.PlayOneShot(_pointSound);
        }    

        public void PlayLoseSound()
        {
            _soundSource.PlayOneShot(_loseSound);
        }
    }

}