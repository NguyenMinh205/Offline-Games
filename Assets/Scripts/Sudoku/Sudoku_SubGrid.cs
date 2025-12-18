using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.Sudoku
{
    public class Sudoku_SubGrid : MonoBehaviour
    {
        [SerializeField] private List<Sudoku_Cell> _cells;
        public List<Sudoku_Cell> Cells { get { return _cells; } }
    }
}
