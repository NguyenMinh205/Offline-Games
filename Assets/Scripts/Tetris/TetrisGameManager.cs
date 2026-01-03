using TMPro;
using UnityEngine;

namespace NguyenQuangMinh.Tetris
{
    public class TetrisGameManager : Singleton<TetrisGameManager>, IGameManager
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _scoreTxt;

        [Header("Settings")]
        [SerializeField] private BoardManager _boardManager;

        public int Score { get; private set; }
        public bool IsGameOver { get; private set; }

        public void StartNewGame()
        {
            Score = 0;
            IsGameOver = false;
            MainGameManager.Instance.ShowScore(true);
            UpdateScoreUI();
            _boardManager.StartNewGame();
        }

        public void Restart()
        {
            StartNewGame();
        }

        public void GameOver()
        {
            IsGameOver = true;
            Debug.Log("Game Over!");
        }

        public void AddScore(int linesCleared)
        {
            Score += linesCleared * (linesCleared * linesCleared) * 10;
            UpdateScoreUI();
        }

        private void UpdateScoreUI()
        {
            if (_scoreTxt != null)
                _scoreTxt.text = Score.ToString();
        }
    }
}