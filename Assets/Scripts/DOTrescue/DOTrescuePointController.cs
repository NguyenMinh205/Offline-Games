using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.DOTrescue
{
    public class DOTrescuePointController : MonoBehaviour
    {
        [SerializeField] private float _rotateSpeed;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                AudioManager.Instance.PlayDotMoveSound();
                _rotateSpeed *= -1f;
            }
        }

        private void FixedUpdate()
        {
            if (DOTrescueGameManager.Instance.IsGameOver || MainGameManager.Instance.GameState != GameState.InGame) return;
            transform.Rotate(0f, 0f, _rotateSpeed * Time.fixedDeltaTime);
        }
    }

}