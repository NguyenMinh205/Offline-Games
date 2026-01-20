using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NguyenQuangMinh.ColorBlock
{
    public class GameManager_ColorBlocks : Singleton<GameManager_ColorBlocks>, IGameManager
    {
        [SerializeField] private ColorBoard_BoardManager boardManager;
        public ColorBoard_BoardManager BoardManager => boardManager;
        [SerializeField] private ColorBlockSpawnBlock spawner;
        public ColorBlockSpawnBlock Spawner => spawner;

        [SerializeField] private int score = 0;
        [SerializeField] private float snapDuration = 0.2f;
        public float SnapDuration => snapDuration;
        private const int scorePerLine = 10;

        public void StartNewGame()
        {
            MainGameManager.Instance.SetHighScore(DataManager.Instance.GameData.ColorBlockHighScore);

            boardManager.InitializeBoard();
            spawner.Init();

            ResetGame();

            spawner.Spawn();
        }

        public void ResetGame()
        {
            MainGameManager.Instance.ShowScore(true);
            score = 0;
            MainGameManager.Instance.UpdateCurScore(score);
            boardManager.ResetBoard();
            spawner.ReturnAllBlocksToPool();
        }

        public void Restart()
        {
            score = 0;
            MainGameManager.Instance.UpdateCurScore(score);
            MainGameManager.Instance.SetHighScore(DataManager.Instance.GameData.ColorBlockHighScore);

            ResetGame();

            spawner.Spawn();
        }

        public void AddScore(int index)
        {
            if (index <= 0) return;

            float multiplier = 1f + (index - 1) * 0.5f;
            float totalScore = index * scorePerLine * multiplier;

            score += Mathf.RoundToInt(totalScore);
            MainGameManager.Instance.UpdateCurScore(score);
            SetHighScore();
        }

        public void GameOver()
        {
            SetHighScore();
            DOVirtual.DelayedCall(1f, () =>
            {
                MainGameManager.Instance.ShowGameOverUI(score);
            });
        }

        public void SetHighScore()
        {
            if (score > DataManager.Instance.GameData.ColorBlockHighScore)
            {
                DataManager.Instance.GameData.ColorBlockHighScore = score;
                MainGameManager.Instance.SetHighScore(score);
                DataManager.Instance.GameData.Save();
            }
        }
    }
}