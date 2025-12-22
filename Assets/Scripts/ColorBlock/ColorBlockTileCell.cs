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

        public void ClearCell()
        {
            image.sprite = null;
            image.color = defaultColor;
            _blockStyle.SetActive(false);
        }

        public void SetBlockSprite(Sprite sprite, Color color)
        {
            image.sprite = sprite;
            image.color = color;
            _blockStyle.SetActive(true);
        }
    }
}