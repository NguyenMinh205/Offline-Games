using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace NguyenQuangMinh.Battleship
{
    public class Cell : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Vector2Int _coordinates;
        public Vector2Int Coordinates => _coordinates;
        [SerializeField] private bool _hasShipPart;
        public bool HasShipPart => _hasShipPart;
        [SerializeField] private bool _isShot;
        public bool IsShot => _isShot;

        [SerializeField] private Image _image;
        public Image Image => _image;
        [SerializeField] private Sprite _wrongPlace;
        [SerializeField] private Sprite _defaultSprite;
        private Color _defaultColor;

        public void Init(int x, int y)
        {
            _defaultColor = _image.color;
            _coordinates = new Vector2Int(x, y);
            _hasShipPart = false;
            _isShot = false;
        }

        public void ResetCell()
        {
            _image.sprite = _defaultSprite;
            _image.color = _defaultColor;
            _hasShipPart = false;
            _isShot = false;
        }    

        public void SetShipPart(bool hasPart)
        {
            _hasShipPart = hasPart;
        }
        public void SetIsShot(bool shot)
        {
            _isShot = shot;
        }

        public void HighLight(Color color)
        {
            _image.color = color;
        }

        public void ChangeTileSprite()
        {
            if (!_isShot) return;

            if (_hasShipPart)
            {
                _image.color = Color.black;
                return;
            }

            _image.sprite = _wrongPlace;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            GameManagerBattleship.Instance.ProcessPlayerAttack(_coordinates);
        }
    }

}