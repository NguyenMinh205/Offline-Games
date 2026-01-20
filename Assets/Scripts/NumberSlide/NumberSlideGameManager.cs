using DG.Tweening;
using NguyenQuangMinh.MemoryCard;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace NguyenQuangMinh.NumberSlide
{
    public class NumberSlideGameManager : Singleton<NumberSlideGameManager>, IGameManager
    {
        [SerializeField] private NumberSlideBoard _board;

        private int _currentScore = 0;

        public void StartNewGame()
        {
            _currentScore = 0;

            MainGameManager.Instance.ShowScore(true);
            MainGameManager.Instance.SetCurScore(_currentScore);
            MainGameManager.Instance.SetHighScore(DataManager.Instance.GameData.NumberSlideHighScore);

            _board.StartNewGame();
        }

        public void ResetGame()
        {
            _board.ClearBoard();
        }

        public void Restart()
        {
            StartNewGame();
        }

        public void GameOver()
        {
            SetHighScore();
            DOVirtual.DelayedCall(1f, () =>
            {
                MainGameManager.Instance.ShowGameOverUI(_currentScore);
            });
        }

        public void PlusScore(int points)
        {
            _currentScore += points;
            MainGameManager.Instance.SetCurScore(_currentScore);
        }

        private void SetHighScore()
        {
            if (_currentScore <= DataManager.Instance.GameData.NumberSlideHighScore)
            {
                return;
            }
            DataManager.Instance.GameData.NumberSlideHighScore = _currentScore;
            DataManager.Instance.GameData.Save();
            MainGameManager.Instance.SetHighScore(_currentScore);
        }
    }
}
