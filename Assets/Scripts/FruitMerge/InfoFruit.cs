using System;
using System.Collections;
using UnityEngine;

namespace NguyenQuangMinh.FruitMerge
{
    public class InfoFruit : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private CircleCollider2D _collider2D;

        private int currentScore = 0;
        private int level;
        public int Level => level;

        private bool isCollider;
        public bool IsCollider => isCollider;

        private Action<InfoFruit, InfoFruit, int> onMerge;
        private Action endGame;
        private Coroutine checkOver;

        private void OnEnable()
        {
            ObserverManager<EventID>.AddRegisterEvent(EventID.UpdateScore, param => UpdateScoreFruit((int)param));
        }

        private void OnDisable()
        {
            ObserverManager<EventID>.RemoveAddListener(EventID.UpdateScore, param => UpdateScoreFruit((int)param));
            StopCheckOver();
        }

        private void UpdateScoreFruit(int value)
        {
            currentScore = value;
        }

        public void Init(int level, Action<InfoFruit, InfoFruit, int> actionMerge, Action endGame, bool isFall = false)
        {
            this.level = level;
            onMerge = actionMerge;
            this.endGame = endGame;
            isCollider = false;

            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;

            rb.bodyType = !isFall ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
        }

        public void OnFall()
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (isCollider) return;

            if (other.gameObject.TryGetComponent(out InfoFruit otherfruit))
            {
                if (level + 1 >= FruitMergeGameManager.Instance.Model.DataFruit.Count) return;

                if (otherfruit.Level == level)
                {
                    if (this.GetInstanceID() > otherfruit.GetInstanceID())
                    {
                        onMerge?.Invoke(this, otherfruit, level + 1);
                        isCollider = true;
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Line"))
            {
                StopCheckOver();
                checkOver = StartCoroutine(CheckGameOver(collision));
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Line"))
            {
                StopCheckOver();
            }
        }

        private void StopCheckOver()
        {
            if (checkOver != null)
            {
                StopCoroutine(checkOver);
                checkOver = null;
            }
        }

        IEnumerator CheckGameOver(Collider2D lineCollider)
        {
            yield return new WaitForSeconds(FruitMergeGameManager.Instance.TimeCheckLose);

            if (lineCollider != null && _collider2D.bounds.Intersects(lineCollider.bounds))
            {
                endGame?.Invoke();
            }
        }
    }
}