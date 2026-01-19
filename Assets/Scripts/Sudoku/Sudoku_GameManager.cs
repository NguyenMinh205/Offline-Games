using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace NguyenQuangMinh.Sudoku
{

    public class Sudoku_GameManager : Singleton<Sudoku_GameManager>, IGameManager
    {
        [SerializeField] private List<Sudoku_SubGrid> _subGrids;

        private bool hasGameFinished;
        private Sudoku_Cell[,] cells;
        private Sudoku_Cell _selectedCell;
        private int _filledCells = 0;

        private const int GRID_SIZE = 9;
        private const int SUBGRID_SIZE = 3;

        public void StartNewGame()
        {
            hasGameFinished = false;
            cells = new Sudoku_Cell[GRID_SIZE, GRID_SIZE];
            _selectedCell = null;
            _filledCells = 0;
            ClearBoard();
            SetupBoard();
        }

        public void Restart()
        {
            ResetDefault(_selectedCell);
            ClearBoard();
            hasGameFinished = false;
            cells = new Sudoku_Cell[GRID_SIZE, GRID_SIZE];
            _selectedCell = null;
            _filledCells = 0;
            SetupBoard();
        }

        public void SetupBoard()
        {
            int[,] puzzleGrid = Generator.GeneratePuzzle(Difficulty.Medium);
            for (int i = 0; i < GRID_SIZE; i++)
            {
                Sudoku_SubGrid subGrid = _subGrids[i];
                List<Sudoku_Cell> subGridCells = subGrid.Cells;
                int subGridRow = i / 3;
                int subGridCol = i % 3;
                int startRow = subGridRow * 3;
                int startCol = subGridCol * 3;
                for (int j = 0; j < GRID_SIZE; j++)
                {
                    subGridCells[j].Row = startRow + j / 3;
                    subGridCells[j].Column = startCol + j % 3;
                    int cellValue = puzzleGrid[subGridCells[j].Row, subGridCells[j].Column];
                    subGridCells[j].Init(cellValue);
                    cells[subGridCells[j].Row, subGridCells[j].Column] = subGridCells[j];
                    if (cellValue != 0) _filledCells++;
                }
            }
        }

        public void OnCellClicked(Sudoku_Cell clickedCell)
        {
            if (_selectedCell != null)
            {
                _selectedCell.BackToDefault();
                ResetDefault(_selectedCell);
            }

            this._selectedCell = clickedCell;
            AudioManager.Instance.PlaySudokuCellClickSound();
            Highlight();
            _selectedCell.Select();
        }    

        public void ResetDefault(Sudoku_Cell cell)
        {
            if (cell == null) return;

            int row = cell.Row;
            int col = cell.Column;

            for (int c = 0; c < GRID_SIZE; c++)
            {
                if (c != col) cells[row, c].BackToDefault();
            }

            for (int r = 0; r < GRID_SIZE; r++)
            {
                if (r != row) cells[r, col].BackToDefault();
            }

            int subRowStart = (row / SUBGRID_SIZE) * SUBGRID_SIZE;
            int subColStart = (col / SUBGRID_SIZE) * SUBGRID_SIZE;
            for (int r = subRowStart; r < subRowStart + SUBGRID_SIZE; r++)
            {
                for (int c = subColStart; c < subColStart + SUBGRID_SIZE; c++)
                {
                    if (r != row || c != col) cells[r, c].BackToDefault();
                }
            }
        }

        public void Highlight()
        {
            if (_selectedCell == null) return;

            int row = _selectedCell.Row;
            int col = _selectedCell.Column;

            for (int c = 0; c < GRID_SIZE; c++)
            {
                if (c != col) cells[row, c].Highlight();
            }

            for (int r = 0; r < GRID_SIZE; r++)
            {
                if (r != row) cells[r, col].Highlight();
            }

            int subRowStart = (row / SUBGRID_SIZE) * SUBGRID_SIZE;
            int subColStart = (col / SUBGRID_SIZE) * SUBGRID_SIZE;
            for (int r = subRowStart; r < subRowStart + SUBGRID_SIZE; r++)
            {
                for (int c = subColStart; c < subColStart + SUBGRID_SIZE; c++)
                {
                    if (r != row || c != col) cells[r, c].Highlight();
                }
            }
        }

        public void InputToCell(int value)
        {
            if (_selectedCell == null || _selectedCell.IsGiven) return;

            int oldValue = _selectedCell.Value;

            if (oldValue == 0 && value != 0)
                _filledCells++;
            else if (oldValue != 0 && value == 0)
                _filledCells--;

            _selectedCell.UpdateValue(value);

            CheckCorrectValue();
            CheckWin();
        }

        public void CheckCorrectValue()
        {
            int row = _selectedCell.Row;
            int col = _selectedCell.Column;
            bool isCorrect = true;

            ClearErrorInArea(row, col);

            isCorrect = isCorrect && CheckLineForConflicts(row, true);

            isCorrect = isCorrect && CheckLineForConflicts(col, false);

            int subRow = (row / 3) * 3;
            int subCol = (col / 3) * 3;
            isCorrect = isCorrect && CheckSubgridForConflicts(subRow, subCol);
            ResetDefault(_selectedCell);
            Highlight();
            if (_selectedCell != null) _selectedCell.Select();

            if (!isCorrect)
            {
                AudioManager.Instance.PlaySudokuNumberWrongSound();
            }
            else
            {
                AudioManager.Instance.PlaySudokuNumberCorrectSound();
            }
        }

        public void ClearErrorInArea(int row, int col)
        {
            for (int c = 0; c < 9; c++)
                cells[row, c].IsIncorrect = false;

            for (int r = 0; r < 9; r++)
                cells[r, col].IsIncorrect = false;

            int sr = (row / 3) * 3;
            int sc = (col / 3) * 3;
            for (int r = sr; r < sr + 3; r++)
                for (int c = sc; c < sc + 3; c++)
                    cells[r, c].IsIncorrect = false;
        }

        private bool CheckLineForConflicts(int index, bool isRow)
        {
            bool hasConflict = false;
            for (int i = 0; i < 9; i++)
            {
                int r = isRow ? index : i;
                int c = isRow ? i : index;
                if (cells[r, c].Value == _selectedCell.Value && (r != _selectedCell.Row || c != _selectedCell.Column))
                {
                    cells[r, c].IsIncorrect = true;
                    cells[_selectedCell.Row, _selectedCell.Column].IsIncorrect = true;
                    hasConflict = true;
                }
            }
            return hasConflict;
        }

        private bool CheckSubgridForConflicts(int startRow, int startCol)
        {
            bool hasConflict = false;
            for (int r = startRow; r < startRow + 3; r++)
            {
                for (int c = startCol; c < startCol + 3; c++)
                {
                    if (cells[r, c].Value == _selectedCell.Value && (r != _selectedCell.Row || c != _selectedCell.Column))
                    {
                        cells[r, c].IsIncorrect = true;
                        cells[_selectedCell.Row, _selectedCell.Column].IsIncorrect = true;
                        hasConflict = true;
                    }
                }
            }
            return hasConflict;
        }

        public void CheckWin()
        {
            if (_filledCells != 81) return;

            for (int r = 0; r < GRID_SIZE; r++)
                for (int c = 0; c < GRID_SIZE; c++)
                    if (cells[r, c].IsIncorrect)
                        return;

            hasGameFinished = true;
            DOVirtual.DelayedCall(1f, () =>
            {
                MainGameManager.Instance.ShowWinUI(false);
            });
            Debug.Log("YOU WIN! Sudoku Solved!");
        }

        public void ClearBoard()
        {
            foreach (Sudoku_SubGrid subGrid in _subGrids)
            {
                foreach (Sudoku_Cell cell in subGrid.Cells)
                {
                    cell.Init('\0');
                    cell.BackToDefault();
                }
            }
        }
    }

}