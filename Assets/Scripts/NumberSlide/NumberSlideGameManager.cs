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
        [SerializeField] private GameObject _scoreUI;
        [SerializeField] private GameObject _highScoreUI;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;

        private int _currentScore = 0;
        private int _highScore = 0;

        public void StartNewGame()
        {
            _currentScore = 0;
            SetScore();

            _board.StartNewGame();
        }

        public void Restart()
        {
            StartNewGame();
        }

        public void GameOver()
        {
            SetHighScore();
            Debug.Log("GameEnd");
        }

        public void PlusScore(int points)
        {
            _currentScore += points;
            SetScore();
        }

        private void SetScore()
        {
            scoreText.text = _currentScore.ToString();
        }

        private void UpdateHighScoreUI()
        {
            highScoreText.text = _highScore.ToString();
        }

        private void SetHighScore()
        {
            if (_currentScore <= _highScore)
            {
                return;
            }
            _highScore = _currentScore;
            UpdateHighScoreUI();
        }
    }
}
