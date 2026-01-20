using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace NguyenQuangMinh.FruitMerge
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Image _nextFruitImg;
        private int score;
        public int Score => score;

        public void Init()
        {
            MainGameManager.Instance.ShowScore(true);
            score = 0;
            MainGameManager.Instance.SetCurScore(score);
            MainGameManager.Instance.SetHighScore(DataManager.Instance.GameData.ColorBlockHighScore);
            ObserverManager<EventID>.AddRegisterEvent(EventID.UpdateScore, param => UpdateViewScore((int)param));
        }

        public void EndGame()
        {
            ObserverManager<EventID>.RemoveAddListener(EventID.UpdateScore, param => UpdateViewScore((int)param));
        }

        private void UpdateViewScore(int value)
        {
            score += value;
            MainGameManager.Instance.SetCurScore(score);
            SetHighScore();
        }

        public void SetNextFruit(Sprite fruitSprite)
        {
            _nextFruitImg.rectTransform.DOScale(Vector3.zero, 0.15f).OnComplete(() =>
            {
                _nextFruitImg.sprite = fruitSprite;
                _nextFruitImg.rectTransform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
            });
        }

        private void SetHighScore()
        {
            if (score <= DataManager.Instance.GameData.NumberSlideHighScore)
            {
                return;
            }
            DataManager.Instance.GameData.NumberSlideHighScore = score;
            DataManager.Instance.GameData.Save();
            MainGameManager.Instance.SetHighScore(score);
        }
    }
}
