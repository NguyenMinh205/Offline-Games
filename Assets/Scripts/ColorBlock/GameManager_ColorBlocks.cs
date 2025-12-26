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
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private float snapDuration = 0.2f;
        public float SnapDuration => snapDuration;
        private const int scorePerLine = 10;

        public void StartNewGame()
        {
            score = 0;
            SetScoreText();

            boardManager.InitializeBoard();
            boardManager.ResetBoard();

            spawner.Init();
            spawner.ReturnAllBlocksToPool();

            spawner.Spawn();
        }

        public void Restart()
        {
            score = 0;
            SetScoreText();

            boardManager.ResetBoard();
            spawner.ReturnAllBlocksToPool();

            spawner.Spawn();
        }

        public void AddScore(int index)
        {
            if (index <= 0) return;

            float multiplier = 1f + (index - 1) * 0.5f;
            float totalScore = index * scorePerLine * multiplier;

            score += Mathf.RoundToInt(totalScore);
            SetScoreText();
        }

        public void SetScoreText()
        {
            scoreText.text = score.ToString();
        }

        public void GameOver()
        {
            Debug.LogError("GAME OVER! Score: " + score);
        }
    }
}