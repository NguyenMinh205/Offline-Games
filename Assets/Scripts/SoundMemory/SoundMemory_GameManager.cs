using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.SoundMemory
{
    public class SoundMemory_GameManager : Singleton<SoundMemory_GameManager>, IGameManager
    {
        [SerializeField] private SoundMemory_SequenceManager _sequenceManager;
        public SoundMemory_SequenceManager Sequence => _sequenceManager;
        [SerializeField] private SoundMemory_UIManager _uiManager;
        public SoundMemory_UIManager UI => _uiManager;
        [SerializeField] private SoundManager_AudioManager _audioManager;
        public SoundManager_AudioManager Audio => _audioManager;
        public float roundDelay = 1f;

        private int score = 0;
        private bool gameActive = true;

        public void StartNewGame()
        {
            _uiManager.Init();
            score = 0;
            gameActive = true;
            _sequenceManager.ResetSequence();
            _uiManager.UpdateScore(score);
            NextRound();
        }

        public void Restart()
        {
            score = 0;
            gameActive = true;
            _sequenceManager.ResetSequence();
            _uiManager.UpdateScore(score);
            NextRound();
        }

        public void NextRound()
        {
            _sequenceManager.AddToSequence();
            StartCoroutine(_sequenceManager.PlaySequenceAnswer(() => _uiManager.EnableButtons(true)));
        }

        public void OnClickButtonSound(int index)
        {
            if (!gameActive) return;

            _audioManager.PlaySound(index);
            _uiManager.AnimateButton(index);

            StartCoroutine(CheckAfterSound(index));
        }

        private IEnumerator CheckAfterSound(int index)
        {
            if (_sequenceManager.CheckPlayerSequence(index))
            {
                if (_sequenceManager.IsRoundComplete())
                {
                    score++;
                    _uiManager.UpdateScore(score);
                    _uiManager.EnableButtons(false);
                    NextRound();
                    yield return null;
                }
            }
            else
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            gameActive = false;
            _uiManager.EnableButtons(false);
            Debug.Log("Game Over! Final Score: " + score);
        }
    }
}