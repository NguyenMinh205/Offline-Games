using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.DOTrescue
{
    public class DOTrescuePoint : MonoBehaviour
    {
        [SerializeField] private GameObject _explosionPrefab;
        [SerializeField] private float _timeParticle = 3f;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Obstacle"))
            {
                AudioManager.Instance.PlayDotColliderSound();
                GameObject explosion = Instantiate(_explosionPrefab, this.transform.position, Quaternion.identity);
                this.gameObject.SetActive(false);
                DOTrescueGameManager.Instance.GameOver();
                DOVirtual.DelayedCall(_timeParticle, () =>
                {
                    Destroy(explosion);
                });
            }
        }
    }
}
