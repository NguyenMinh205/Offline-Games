using DG.Tweening;
using NguyenQuangMinh.FruitMerge;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.FruitMerge
{
    public class FruitMergeGameManager : Singleton<FruitMergeGameManager>, IGameManager
    {
        [Space]
        [Header("Set up")]
        [SerializeField] private ModelFruits model;
        [SerializeField] private UIController _uiFruitMerge;

        public ModelFruits Model
        {
            get => model;
        }

        [SerializeField] private InfoFruit _curFruit;
        [SerializeField] private Transform objectSpawn;
        [SerializeField] private Transform objectPool;
        [SerializeField] private ParticleSystem effectMerge;
        [SerializeField] private float posValid;
        [SerializeField] private GameObject scoreLine;
        [SerializeField] private int _scoreEachLevel;
        [SerializeField] List<InfoFruit> allFruits;

        private bool canSwipe;
        private bool isDelay;
        private int indexNextFruit;

        [Space]
        [Header("Time Spawn")]
        [SerializeField] private float timeSpawn;
        [SerializeField] private float _timeCheckLose;
        public float TimeCheckLose => _timeCheckLose;

        [Space]
        [Header("Game State")]
        [SerializeField] private bool _isLose;
        public bool IsLose
        {
            get => _isLose;
            set => _isLose = value;
        }

        public void StartNewGame()
        {
            _isLose = false;
            if (_curFruit != null)
            {
                PoolingManager.Despawn(_curFruit.gameObject);
            }
            _curFruit = null;
            _uiFruitMerge.Init();

            foreach (var item in allFruits)
            {
                if (item != null && item.gameObject.activeSelf)
                    PoolingManager.Despawn(item.gameObject);
            }
            allFruits.Clear();

            if (_curFruit != null)
            {
                PoolingManager.Despawn(_curFruit.gameObject);
                _curFruit = null;
            }

            MainGameManager.Instance.ShowScore(true);
            MainGameManager.Instance.SetHighScore(DataManager.Instance.GameData.ColorBlockHighScore);

            NextFruit();
            SpawnFruit();
        }

        public void Restart()
        {
            StartNewGame();
        }

        private void Update()
        {
            if (_isLose && isDelay) return;
            if (Input.GetMouseButtonDown(0))
            {
                OnClick();
            }
            if (Input.GetMouseButton(0))
            {
                OnMove();
            }
            if (Input.GetMouseButtonUp(0))
            {
                OnDown();
            }

            if (_isLose)
            {
                GameOver();
            }
        }

        private void OnClick()
        {
            if (!isDelay)
            {
                canSwipe = true;
                MoveObject(Input.mousePosition);
                scoreLine.SetActive(true);
            }
        }

        private void OnMove()
        {
            if (canSwipe)
            {
                MoveObject(Input.mousePosition);
            }
        }

        private void OnDown()
        {
            if (!canSwipe) return;
            if (_curFruit != null)
            {
                _curFruit.OnFall();
                allFruits.Add(_curFruit);
                _curFruit.transform.SetParent(objectPool);
                _curFruit = null;
            }

            scoreLine.SetActive(false);
            AudioManager.Instance.PlayFruitDropSound();
            isDelay = true;
            canSwipe = false;

            DOVirtual.DelayedCall(timeSpawn, delegate
            {
                if (!_isLose)
                {
                    SpawnFruit();
                    isDelay = false;
                }
            });
        }

        private void MoveObject(Vector3 screenPos)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            float xPos = Mathf.Clamp(worldPos.x, -posValid, posValid);
            objectSpawn.transform.position = new Vector3(xPos, objectSpawn.transform.position.y, 0f);
        }

        private void NextFruit()
        {
            indexNextFruit = Random.Range(0, model.LimitFruit);
            _uiFruitMerge.SetNextFruit(model.DataFruit[indexNextFruit].GetComponentInChildren<SpriteRenderer>().sprite);
        }

        private void SpawnFruit()
        {
            if (_isLose) return;
            _curFruit = PoolingManager.Spawn(model.DataFruit[indexNextFruit], objectSpawn.position, Quaternion.identity, objectSpawn);
            AudioManager.Instance.PlayFruitSpawnSound();
            _curFruit.Init(indexNextFruit, MergeFruit, GameOver);
            _curFruit.transform.localScale = Vector3.zero;
            _curFruit.transform.DOScale(Vector3.one, 0.2f);
            NextFruit();
        }

        private void MergeFruit(InfoFruit fruit1, InfoFruit fruit2, int level)
        {
            if (fruit1.IsCollider && fruit2.IsCollider) return;

            Vector3 newPosSpawn = (fruit1.transform.position + fruit2.transform.position) / 2;

            if (effectMerge != null)
            {
                ParticleSystem effect = PoolingManager.Spawn(effectMerge, newPosSpawn, Quaternion.identity, objectPool);
                effect.Play();
                AudioManager.Instance.PlayFruitMergeSound();
                DOVirtual.DelayedCall(1.25f, () => PoolingManager.Despawn(effect.gameObject));
            }

            PoolingManager.Despawn(fruit1.gameObject);
            PoolingManager.Despawn(fruit2.gameObject);
            allFruits.Remove(fruit1);
            allFruits.Remove(fruit2);

            InfoFruit newFruit = PoolingManager.Spawn(model.DataFruit[level], newPosSpawn, Quaternion.identity, objectPool);
            newFruit.Init(level, MergeFruit, GameOver, true);
            
            newFruit.transform.localScale = Vector3.zero;
            newFruit.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBounce);

            allFruits.Add(newFruit);

            ObserverManager<EventID>.PostEvent(EventID.UpdateScore, _scoreEachLevel * (level + 1));
        }

        public void GameOver()
        {
            if (_curFruit != null)
                PoolingManager.Despawn(_curFruit.gameObject);
           _isLose = true;
            _uiFruitMerge.EndGame();
            int score = _uiFruitMerge.Score;

            if (score > DataManager.Instance.GameData.FruitMergeHighScore)
            {
                DataManager.Instance.GameData.FruitMergeHighScore = score;
                DataManager.Instance.GameData.Save();
            }

            DOVirtual.DelayedCall(1f,() =>
            {
                MainGameManager.Instance.ShowGameOverUI(score);
            });
        }

        private void OnDisable()
        {
            _uiFruitMerge.EndGame();
        }
    }
}