using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.ColorConnect
{
    public enum TypeOfColor
    {
        None, Red, Green, Blue, Orange, Gray, Yellow, Brown, Purple, Pink
    }

    [System.Serializable]
    public class DataColor
    {
        public TypeOfColor type;
        public Color color;
    }

    [CreateAssetMenu(fileName = "ColorPalette", menuName = "ScriptableObject/ColorConnect/ColorPalette")]
    public class ColorPalette : ScriptableObject
    {
        [SerializeField] private List<DataColor> _colors;
        public Color GetColorByType(TypeOfColor type)
        {
            foreach (DataColor data in _colors)
            {
                if (data.type == type) return data.color;
            }
            return Color.white;
        }
    }
}