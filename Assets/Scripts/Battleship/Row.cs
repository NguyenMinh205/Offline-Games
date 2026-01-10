using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.Battleship
{
    public class Row : MonoBehaviour
    {
        [SerializeField] private List<Cell> _cells;
        public List<Cell> Cells => _cells;
    }
}
