using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.DOTrescue
{
    public class DOTrescuePoint : MonoBehaviour
    {
        [SerializeField] private GameObject _explosionPrefab;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Obstacle"))
            {
                DOTrescueSoundManager.Instance.PlayLoseSound();
                Instantiate(_explosionPrefab, this.transform.position, Quaternion.identity);
                Destroy(gameObject);
                DOTrescueGameManager.Instance.GameOver();
            }
        }
    }
}
