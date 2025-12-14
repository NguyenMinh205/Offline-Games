using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.NumberSlide
{
    [CreateAssetMenu(fileName = "NumberSlideCellState", menuName = "ScriptableObject/NumberSlide/NumberSlideCellState")]
    public class NumberSlideCellState : ScriptableObject 
    {
        public int number;
        public Color bgColor;
        public Color textColor;
    }
}
