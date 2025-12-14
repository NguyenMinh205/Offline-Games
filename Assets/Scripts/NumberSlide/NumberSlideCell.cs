using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.NumberSlide
{
    public class NumberSlideCell : MonoBehaviour
    {
        public Vector2Int MatrixIndices { get; set; }
        public NumberSlideTile Tile { get; set; }
        public bool Empty => Tile == null;
        public bool Occupied => Tile != null;
    }
}
