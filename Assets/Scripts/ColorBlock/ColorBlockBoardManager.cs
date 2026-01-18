using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.ColorBlock
{
    public class ColorBoard_BoardManager : Singleton<ColorBoard_BoardManager>
    {
        [SerializeField] private int amountColumn = 8;
        [SerializeField] private int amountRow = 8;
        private ColorBlockTileCell[,] grid;
        private List<ColorBlockTileCell> cells;

        public int AmountColumn => amountColumn;
        public int AmountRow => amountRow;
        public List<ColorBlockTileCell> AllCells => cells;

        public void InitializeBoard()
        {
            grid = new ColorBlockTileCell[amountRow, amountColumn];
            cells = new List<ColorBlockTileCell>(GetComponentsInChildren<ColorBlockTileCell>());

            if (cells.Count != amountRow * amountColumn)
            {
                return;
            }

            InitGrid();
        }

        public void InitGrid()
        {
            int index = 0;
            for (int x = 0; x < amountRow; x++)
            {
                for (int y = 0; y < amountColumn; y++)
                {
                    cells[index].Init();
                    grid[x, y] = cells[index];
                    grid[x, y].coordinates = new Vector2Int(x, y);
                    index++;
                }
            }
        }

        public void CheckGrid()
        {
            List<int> rowsToClear = new List<int>();
            List<int> colsToClear = new List<int>();

            for (int row = 0; row < amountRow; row++)
            {
                if (IsFullRow(row)) rowsToClear.Add(row);
            }

            for (int col = 0; col < amountColumn; col++)
            {
                if (IsFullColumn(col)) colsToClear.Add(col);
            }

            foreach (int row in rowsToClear) ClearRow(row);
            foreach (int col in colsToClear) ClearColumn(col);

            int totalCleared = rowsToClear.Count + colsToClear.Count;
            if (totalCleared > 0)
            {
                GameManager_ColorBlocks.Instance.AddScore(totalCleared);
                AudioManager.Instance.PlayColorBlockClearSound();
            }
            else
            {
                AudioManager.Instance.PlayColorBlockPlaceSound();
            }

            GameManager_ColorBlocks.Instance.Spawner.CheckAndSpawn();

            if (!CanPlaceAnyBlock())
                GameManager_ColorBlocks.Instance.GameOver();
        }

        private bool IsFullRow(int row)
        {
            for (int col = 0; col < amountColumn; col++)
            {
                if (grid[row, col].IsEmpty) return false;
            }
            return true;
        }

        private bool IsFullColumn(int column)
        {
            for (int row = 0; row < amountRow; row++)
            {
                if (grid[row, column].IsEmpty) return false;
            }
            return true;
        }

        private void ClearRow(int row)
        {
            for (int col = 0; col < amountColumn; col++)
            {
                grid[row, col].ClearCell();
            }
        }

        private void ClearColumn(int column)
        {
            for (int row = 0; row < amountRow; row++)
            {
                grid[row, column].ClearCell();
            }
        }

        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < amountRow && y >= 0 && y < amountColumn;
        }

        public bool CanPlaceAnyBlock()
        {
            foreach (var slot in GameManager_ColorBlocks.Instance.Spawner.Slots)
            {
                if (slot.IsEmpty() || slot.GetBlock() == null) continue;

                ColorBlock_Block block = slot.GetBlock();

                for (int x = 0; x < amountRow; x++)
                {
                    for (int y = 0; y < amountColumn; y++)
                    {
                        Debug.Log("Detact: " + x + ", " + y);
                        if (CanPlaceBlockAt(block, x, y))
                            return true;
                    }
                }
            }
            return false;
        }

        private int ConvertX(float x)
        {
            if (Mathf.Approximately(x, 0f)) return 0;

            if (x < 0)
            {
                return Mathf.FloorToInt(x - 0.5f);
            }
            else
            {
                return Mathf.CeilToInt(x + 0.5f);
            }
        }

        private int ConvertY(float y)
        {
            if (Mathf.Approximately(y, 0f)) return 0;

            int raw;
            if (y > 0)
            {
                raw = Mathf.FloorToInt(-(y + 0.5f));
            }
            else
            {
                raw = Mathf.CeilToInt(-(y - 0.5f));
            }

            return raw;
        }


        private bool CanPlaceBlockAt(ColorBlock_Block block, int startX, int startY)
        {
            foreach (var piece in block.Tiles)
            {
                float px = piece.transform.localPosition.x;
                float py = piece.transform.localPosition.y;

                int offsetX = ConvertX(px);
                int offsetY = ConvertY(py);

                int targetX = startX + offsetX;
                int targetY = startY + offsetY;

                Debug.Log($"Piece local: ({px}, {py}) → Offset: ({offsetX}, {offsetY}) → Target: ({targetX}, {targetY})");

                if (!IsValidPosition(targetX, targetY) || !grid[targetX, targetY].IsEmpty)
                {
                    return false;
                }
            }
            return true;
        }

        public void ResetBoard()
        {
            if (grid == null) return;

            for (int x = 0; x < amountRow; x++)
            {
                for (int y = 0; y < amountColumn; y++)
                {
                    if (grid[x, y] != null)
                    {
                        grid[x, y].ClearCell();
                        grid[x, y].HighlightCell(false);
                    }
                }
            }
        }

    }
}