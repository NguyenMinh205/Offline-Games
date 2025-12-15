using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.MineSweeper
{
    public class Tile_MineSweeper : MonoBehaviour
    {
        [Header("Sprites")]
        [SerializeField] private Sprite _unclickedTile;
        [SerializeField] private Sprite _mineSprite;
        [SerializeField] private Sprite _mineHitSprite;
        [SerializeField] private Sprite _flagSprite;
        [SerializeField] private List<Sprite> _numberSprites;

        [Header("Info Tile")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private int _mineCount = 0;
        private bool _isMine;
        private bool _isFlagged;
        private bool _isRevealed;

        public int MineCount
        {
            get { return _mineCount; }
            set { _mineCount = value; }
        }

        public bool IsFlagged
        {
            get { return _isFlagged; }
            set { _isFlagged = value; }
        }

        public bool IsMine
        {
            get { return _isMine; }
            set { _isMine = value; }
        }

        public bool IsRevealed
        {
            get { return _isRevealed; }
            set { _isRevealed = value; }
        }

        private void Awake()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Init()
        {
            _isFlagged = false;
            _isRevealed = false;
            _isMine = false;
            _mineCount = 0;
            _spriteRenderer.sprite = _unclickedTile;
        }

        private void OnMouseOver()
        {
            if (!_isRevealed)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    MineSweeper_GameManager.Instance.HandleTileClick(this);
                }
            }
        }

        public void SetFlagSpriteDirect()
        {
            _isFlagged = true;
            _spriteRenderer.sprite = _flagSprite;
        }

        public void SetUnclickedSpriteDirect()
        {
            _isFlagged = false;
            _spriteRenderer.sprite = _unclickedTile;
        }

        public void RevealTile()
        {
            if (_isRevealed || _isFlagged)
                return;

            _isRevealed = true;

            if (_isMine)
            {
                _spriteRenderer.sprite = _mineHitSprite;
                MineSweeper_GameManager.Instance.GameOver();
                return;
            }

            if (_mineCount > 0)
            {
                if (_mineCount - 1 >= 0 && _mineCount - 1 < _numberSprites.Count)
                    _spriteRenderer.sprite = _numberSprites[_mineCount - 1];
                else
                    _spriteRenderer.sprite = null;
            }
            else
            {
                _spriteRenderer.sprite = null;
            }
        }

        public void ShowMineSprite()
        {
            _spriteRenderer.sprite = _mineSprite;
        }
    }
}
