using DG.Tweening;
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
            IsGameOver = false;
            _boardManager.StartNewGame();
        }

        public void ResetGame()
        {
            MainGameManager.Instance.ShowScore(true);
            Score = 0;
            MainGameManager.Instance.UpdateCurScore(Score);
            MainGameManager.Instance.SetHighScore(DataManager.Instance.GameData.TetrisHighScore);
            _boardManager.ResetBoard();
        }

        public void Restart()
        {
            StartNewGame();
        }

        public void GameOver()
        {
            IsGameOver = true;
            SetHighScore();
            DOVirtual.DelayedCall(1f, () =>
            {
                MainGameManager.Instance.ShowGameOverUI(Mathf.FloorToInt(Score));
            });
        }

        public void AddScore(int linesCleared)
        {
            Score += linesCleared * (linesCleared * linesCleared) * 10;
            MainGameManager.Instance.UpdateCurScore(Score);

            if (Score > DataManager.Instance.GameData.TetrisHighScore)
            {
                MainGameManager.Instance.SetHighScore(Score);
            }
        }

        public void SetHighScore()
        {
            if (Score <= DataManager.Instance.GameData.TetrisHighScore)
            {
                return;
            }
            DataManager.Instance.GameData.TetrisHighScore = Score;
            DataManager.Instance.GameData.Save();
            MainGameManager.Instance.SetHighScore(Score);
        }
    }
}