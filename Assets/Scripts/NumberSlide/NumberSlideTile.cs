using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace NguyenQuangMinh.NumberSlide
{
    public class NumberSlideTile : MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private TextMeshProUGUI _text;
        private NumberSlideCellState _state;
        public NumberSlideCellState State => _state;
        private NumberSlideCell _cell;
        public NumberSlideCell Cell => _cell;
        private bool _merged;
        public bool Merged
        {
            get { return _merged; }
            set { _merged = value; }
        }

        public void ResetMerged()
        {
            _merged = false;
        }

        public void Initialize(NumberSlideCellState tileState)
        {
            _state = tileState;
            _background.color = tileState.bgColor;
            _text.color = tileState.textColor;
            _text.text = tileState.number.ToString();
        }

        public void SetTileCell(NumberSlideCell newCell)
        {
            if (this.Cell != null)
            {
                this.Cell.Tile = null;
            }

            _cell = newCell;
            _cell.Tile = this;
        }

        public void MoveToCell(float duration)
        {
            if (_cell != null)
            {
                transform.DOMove(_cell.transform.position, duration);
            }
        }

        public void SnapToCell()
        {
            if (_cell != null)
            {
                transform.position = _cell.transform.position;
            }
        }

        public void Merge(NumberSlideCell otherCell)
        {
            if (this.Cell != null)
            {
                this.Cell.Tile = null;
            }
            _cell = null;
            transform.DOMove(otherCell.transform.position, 0.15f).OnComplete(() => {
                PoolingManager.Despawn(this.gameObject);
            });
        }
        public void PlayMergeAnimation()
        {
            transform.DOScale(1.2f, 0.1f).OnComplete(() => {
                transform.DOScale(1f, 0.1f);
            });
        }
    }
}