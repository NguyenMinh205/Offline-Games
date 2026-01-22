using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NguyenQuangMinh.ColorBlock
{
    public class ColorBlockTileCell : MonoBehaviour
    {
        public Vector2Int coordinates { get; set; }

        public bool IsEmpty => image.sprite == null;

        [SerializeField] private Image image;
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color highlightColor;
        [SerializeField] private GameObject _blockStyle;

        public void Init()
        {
            image.sprite = null;
            image.color = defaultColor;
            _blockStyle.SetActive(false);

            image.transform.localScale = Vector3.one;
        }

        public void HighlightCell(bool highlight)
        {
            if (IsEmpty)
            {
                image.color = highlight ? highlightColor : defaultColor;
            }
        }

        public void HighlightCellWithColor(Color blockColor)
        {
            if (IsEmpty)
            {
                blockColor.a = 0.5f;
                image.color = blockColor;
            }
        }

        public void PlayClearAnimation(float delay)
        {
            image.transform.DOKill();

            image.transform.DOScale(Vector3.zero, 0.25f)
                .SetDelay(delay)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    ClearCell();
                });
        }

        public void ClearCell()
        {
            image.sprite = null;
            image.color = defaultColor;
            _blockStyle.SetActive(false);

            image.transform.localScale = Vector3.one;
        }

        public void SetBlockSprite(Sprite sprite, Color color)
        {
            image.transform.localScale = Vector3.one;
            image.transform.DOKill();

            image.sprite = sprite;
            image.color = color;
            _blockStyle.SetActive(true);
        }
    }
}