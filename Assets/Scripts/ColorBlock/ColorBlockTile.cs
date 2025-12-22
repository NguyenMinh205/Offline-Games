using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.ColorBlock
{
    public class ColorBlockTile : MonoBehaviour
    {
        [SerializeField] private DragAndDropBlock dragDrop;
        [SerializeField] private SpriteRenderer spriteRenderer;

        public DragAndDropBlock DragDrop => dragDrop;
        public SpriteRenderer SpriteRenderer => spriteRenderer;
    }
}