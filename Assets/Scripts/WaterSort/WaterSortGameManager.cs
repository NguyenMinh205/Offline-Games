using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.WaterSort
{
    public enum WaterSortEvent
    {
        OnTransferFinished,
        OnLevelCompleted
    }

    public class WaterSortGameManager : Singleton<WaterSortGameManager>, IGameManager
    {
        [Header("Global Settings")]
        [SerializeField] private LineRenderer _waterLineRenderer;
        [SerializeField] private Transform _parent;

        [Header("Grid Layout Settings")]
        [SerializeField] private float _gridStartX = -2.1f;
        [SerializeField] private float _gridStartY = 1.5f;
        [SerializeField] private float _gridSpacingX = 1.4f;
        [SerializeField] private float _gridSpacingY = 2.5f;
        [SerializeField] private int _maxColumns = 4;

        [Header("References")]
        [SerializeField] private BottleController _bottlePrefab;
        [SerializeField] private List<DataLevel> dataLevels = new List<DataLevel>();

        [Header("Runtime")]
        private List<BottleController> _listBottle = new List<BottleController>();
        private BottleController _chosenBottle;
        private BottleController _targetBottle;

        private int _currentLevelIndex = 0;
        private bool _isGameActive = false;

        private void OnEnable()
        {
            ObserverManager<WaterSortEvent>.AddRegisterEvent(WaterSortEvent.OnTransferFinished, OnTransferFinishedHandler);
        }

        private void OnDisable()
        {
            ObserverManager<WaterSortEvent>.RemoveAddListener(WaterSortEvent.OnTransferFinished, OnTransferFinishedHandler);
        }

        public void StartNewGame()
        {
            _currentLevelIndex = 0;
            LoadLevel(_currentLevelIndex);
        }

        public void ResetGame()
        {
            if (_listBottle.Count > 0)
            {
                foreach (var bottle in _listBottle)
                {
                    PoolingManager.Despawn(bottle.gameObject);
                }
                _listBottle.Clear();
            }
        }

        public void Restart()
        {
            LoadLevel(_currentLevelIndex);
        }

        public void UpdateWaterLine(bool isActive, Vector3 startPos, Vector3 endPos, Color color)
        {
            if (_waterLineRenderer == null) return;

            if (isActive)
            {
                _waterLineRenderer.gameObject.SetActive(true);
                _waterLineRenderer.startColor = color;
                _waterLineRenderer.endColor = color;
                _waterLineRenderer.SetPosition(0, startPos);
                _waterLineRenderer.SetPosition(1, endPos);
            }
            else
            {
                _waterLineRenderer.gameObject.SetActive(false);
            }
        }

        public void UpdateWaterLine(bool isActive)
        {
            if (_waterLineRenderer != null) _waterLineRenderer.gameObject.SetActive(isActive);
        }

        private void LoadLevel(int levelIndex)
        {
            UpdateWaterLine(false);
            _chosenBottle = null;
            _targetBottle = null;

            int actualDataIndex = levelIndex % dataLevels.Count;
            DataLevel currentData = dataLevels[actualDataIndex];

            for (int i = 0; i < currentData.data.Count; i++)
            {
                int row = i / _maxColumns;
                int col = i % _maxColumns;

                float posX = _gridStartX + (col * _gridSpacingX);
                float posY = _gridStartY - (row * _gridSpacingY);
                Vector3 spawnPos = new Vector3(posX, posY, 0);

                BottleController newBottle = PoolingManager.Spawn(_bottlePrefab, spawnPos, Quaternion.identity, _parent);
                newBottle.SetupBottle(currentData.data[i]);

                _listBottle.Add(newBottle);
            }

            _isGameActive = true;
        }

        private void Update()
        {
            if (_isGameActive && Input.GetMouseButtonDown(0))
            {
                HandleInput();
            }
        }

        private void HandleInput()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D raycast2D = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (raycast2D.collider != null && raycast2D.collider.TryGetComponent<BottleController>(out BottleController hit))
            {
                if (hit.IsCompleted) return;

                if (_chosenBottle == null)
                {
                    if (hit.NumOfColorInBottle > 0)
                    {
                        AudioManager.Instance.PlayWaterSortSelectedBottleSound();
                        _chosenBottle = hit;

                        _chosenBottle.SetSelected(true);
                    }
                }
                else
                {
                    if (_chosenBottle == hit)
                    {
                        AudioManager.Instance.PlayWaterSortUnselectedBottleSound();

                        _chosenBottle.SetSelected(false);
                        _chosenBottle = null;
                    }
                    else
                    {
                        _targetBottle = hit;
                        _chosenBottle.BottleControllerRef = _targetBottle;

                        _chosenBottle.SetTopColorValue();
                        _targetBottle.SetTopColorValue();

                        if (_targetBottle.CheckFillColor(_chosenBottle.TopColor))
                        {
                            _isGameActive = false;
                            _chosenBottle.StartTransferColor();
                        }
                        else
                        {
                            AudioManager.Instance.PlayWaterSortUnselectedBottleSound();
                            _chosenBottle.SetSelected(false);

                            _chosenBottle = null;
                            _targetBottle = null;
                        }
                    }
                }
            }
            else
            {
                if (_chosenBottle != null)
                {
                    _chosenBottle.SetSelected(false);
                    _chosenBottle = null;
                }
            }
        }

        private void OnTransferFinishedHandler(object param)
        {
            _isGameActive = true;
            CheckWinCondition();
        }

        private void CheckWinCondition()
        {
            bool isAllCompleted = true;

            foreach (var bottle in _listBottle)
            {
                if (bottle.NumOfColorInBottle == 0) continue;

                if (!bottle.IsCompleted)
                {
                    isAllCompleted = false;
                    break;
                }
            }

            if (isAllCompleted)
            {
                Debug.Log("WIN LEVEL!");
                _isGameActive = false;
                MainGameManager.Instance.ShowWinUI(true, () =>
                {
                    _currentLevelIndex++;
                    LoadLevel(_currentLevelIndex);
                });
            }
        }
    }
}