using UnityEngine;

namespace NguyenQuangMinh.WaterSort
{
    [CreateAssetMenu(fileName = "NewColor", menuName = "ScriptableObject/WaterSort/Color Data")]
    public class BottleColorSO : ScriptableObject
    {
        public string colorName;
        public Color colorValue;
    }
}