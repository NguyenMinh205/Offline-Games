using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.DOTrescue
{
    public class DOTrescueObstacleController : MonoBehaviour
    {
        [SerializeField] private float _minRotateSpeed, _maxRotateSpeed;
        [SerializeField] private float _minRotateTime, _maxRotateTime;

        private float _originalMinSpeed;
        private float _originalMaxSpeed;

        private float _currentRotateSpeed;
        private float _rotateTime;
        private float _currentRotateTime;

        private void Awake()
        {
            _originalMinSpeed = _minRotateSpeed;
            _originalMaxSpeed = _maxRotateSpeed;
        }

        private void Update()
        {
            if (DOTrescueGameManager.Instance.IsGameOver) return;
            _currentRotateTime += Time.deltaTime;

            if (_currentRotateTime > _rotateTime)
            {
                SetSpeedAndTime();
            }
        }

        private void FixedUpdate()
        {
            if (DOTrescueGameManager.Instance.IsGameOver) return;
            transform.Rotate(0f, 0f, _currentRotateSpeed * Time.fixedDeltaTime);
        }

        public void SetSpeedAndTime()
        {
            _currentRotateTime = 0f;
            float speedMag = Random.Range(_minRotateSpeed, _maxRotateSpeed);
            float direction = Random.Range(0, 2) == 0 ? 1f : -1f;
            _currentRotateSpeed = speedMag * direction;
            _rotateTime = Random.Range(_minRotateTime, _maxRotateTime);
        }

        public void IncreaseDifficulty(float percent)
        {
            _minRotateSpeed *= (1f + percent);
            _maxRotateSpeed *= (1f + percent);
            _currentRotateSpeed *= (1f + percent);

            Debug.Log($"Speed Increased! New Range: {_minRotateSpeed} - {_maxRotateSpeed}");
        }

        public void ResetDifficulty()
        {
            _minRotateSpeed = _originalMinSpeed;
            _maxRotateSpeed = _originalMaxSpeed;
            SetSpeedAndTime();
        }
    }
}