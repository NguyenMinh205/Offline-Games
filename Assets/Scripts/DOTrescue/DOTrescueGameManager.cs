using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NguyenQuangMinh.DOTrescue
{
    public class DOTrescueGameManager : Singleton<DOTrescueGameManager>, IGameManager
    {
        [SerializeField] private DOTrescueObstacleController _obstacleController;
        [SerializeField] private TextMeshProUGUI _scoreTxt;
        [SerializeField] private List<int> _levelSpeed, levelMax;
        [SerializeField] private float _levelIncreaseSpeed = 0.15f;
        [SerializeField] private GameObject _point;
        [SerializeField] private GameObject _player;
        [SerializeField] private GameObject _obstacle;

        private bool _isGameOver = false;
        public bool IsGameOver => _isGameOver;
        private float _score = 0f;
        private float _scoreSpeed;
        private int _curLevelIndex;

        public void StartNewGame()
        {
            _isGameOver = false;
            _score = 0f;
            _curLevelIndex = 0;

            _obstacleController.ResetDifficulty();

            if (_levelSpeed.Count > 0)
                _scoreSpeed = _levelSpeed[_curLevelIndex];

            _point.SetActive(true);
            _player.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            _obstacle.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            _player.gameObject.SetActive(true);
            _obstacle.gameObject.SetActive(true);
            MainGameManager.Instance.ShowScore(true);
            MainGameManager.Instance.UpdateCurScore((int)_score);
            MainGameManager.Instance.SetHighScore(DataManager.Instance.GameData.DotRescueHighScore);
            MainGameManager.Instance.CountDown();
        }

        public void Restart()
        {
            StartNewGame();
        }

        private void Update()
        {
            if (_isGameOver || MainGameManager.Instance.GameState != GameState.InGame) return;

            _score += _scoreSpeed * Time.deltaTime;
            MainGameManager.Instance.UpdateCurScore(Mathf.FloorToInt(_score));

            if (_curLevelIndex < levelMax.Count && _score > levelMax[_curLevelIndex])
            {
                if (_curLevelIndex < _levelSpeed.Count - 1)
                {
                    _curLevelIndex++;
                    _scoreSpeed = _levelSpeed[_curLevelIndex];

                    _obstacleController.IncreaseDifficulty(_levelIncreaseSpeed);
                }
            }
        }

        public void GameOver()
        {
            _isGameOver = true;
            if (_score > DataManager.Instance.GameData.DotRescueHighScore)
            {
                DataManager.Instance.GameData.DotRescueHighScore = Mathf.FloorToInt(_score);
                DataManager.Instance.GameData.Save();
            }

            DOVirtual.DelayedCall(1f, () =>
            {
                MainGameManager.Instance.ShowGameOverUI(Mathf.FloorToInt(_score));
            });
        }

        private void OnDisable()
        {
            _player.gameObject.SetActive(false);
            _obstacle.gameObject.SetActive(false);
        }
    }
}