using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.NumberSlide
{
    public class NumberSlideRow : MonoBehaviour
    {
        [SerializeField] private List<NumberSlideCell> _cells;
        public List<NumberSlideCell> Cells => _cells;
    }
}
