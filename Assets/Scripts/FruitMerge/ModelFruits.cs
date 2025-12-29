using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.FruitMerge
{
    public class ModelFruits : MonoBehaviour
    {
        [SerializeField] private int limitFruit;
        [SerializeField] private List<InfoFruit> dataFruit;

        public int LimitFruit
        {
            get => limitFruit;
            set => limitFruit = value;
        }

        public List<InfoFruit> DataFruit
        {
            get => dataFruit;
            set => dataFruit = value;
        }
    }
}
