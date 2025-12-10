using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.TicTacToe
{
    public class TicTacToeGridManager : MonoBehaviour
    {
        [SerializeField] private List<TicTacToeCellController> _grid;
        public List<TicTacToeCellController> Grid => _grid;

        public void SetupGrid()
        {
            for (int i = 0; i < _grid.Count; i++)
            {
                _grid[i].CellID = i;
                _grid[i].SetState(CellState.Empty);
            }
        }    
        
        public void SetSpecificCell(CellState cellState, int cellIndex)
        {
            _grid[cellIndex].SetState(cellState);
        }

        public TicTacToeCellController GetCellControllerAt(int index)
        {
            if (index < 0 || index >= _grid.Count)
            {
                return null;
            }
            return _grid[index];
        }

        public bool CheckFullGrid()
        {
            foreach (var cell in _grid)
            {
                if (cell.CurrentState == CellState.Empty)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
