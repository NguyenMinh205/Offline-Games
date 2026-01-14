using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.WaterSort
{
    [CreateAssetMenu(fileName = "NewLevelWaterSort", menuName = "ScriptableObject/WaterSort/Level Data")]
    public class DataLevel : ScriptableObject
    {
        public List<DataWaterSortObject> data;
    }

    [System.Serializable]
    public class DataWaterSortObject
    {
        [Range(0f, 4f)]
        public int numOfColorInBottle = 4;
        public List<BottleColorSO> _colorsBottle;
    }

}