using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace NguyenQuangMinh.NumberSlide
{
    public class NumberSlideGrid : MonoBehaviour
    {
        [SerializeField] private List<NumberSlideRow> _rows;
        public List<NumberSlideRow> Rows => _rows;
        private int size => _rows.Count * _rows[0].Cells.Count;
        public int GetSize()
        {
            return size;
        }
        private int height => _rows.Count;
        public int GetHeight()
        {
            return height;
        }
        private int width => size / height;
        public int GetWidth()
        {
            return width;
        }

        private void Start()
        {
            for (int y = 0; y < _rows.Count; y++)
            {
                for (int x = 0; x < _rows[y].Cells.Count; x++)
                {
                    _rows[y].Cells[x].MatrixIndices = new Vector2Int(x, y);
                }
            }
        }

        public NumberSlideCell GetCell(Vector2Int coordinates)
        {
            return GetCell(coordinates.x, coordinates.y);
        }

        public NumberSlideCell GetCell(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                return _rows[y].Cells[x];
            }
            else
            {
                return null;
            }
        }

        public NumberSlideCell GetAdjacentCell(NumberSlideCell cell, Vector2Int direction)
        {
            Vector2Int coordinates = cell.MatrixIndices;
            coordinates.x += direction.x;
            coordinates.y -= direction.y;

            return GetCell(coordinates);
        }

        public NumberSlideCell GetRandomEmptyCell()
        {
            List<NumberSlideCell> emptyCells = new List<NumberSlideCell>();

            foreach (NumberSlideRow row in _rows)
            {
                foreach (NumberSlideCell cell in row.Cells)
                {
                    if (cell.Empty)
                    {
                        emptyCells.Add(cell);
                    }
                }
            }

            if (emptyCells.Count == 0)
            {
                return null;
            }

            int randomIndex = Random.Range(0, emptyCells.Count);
            return emptyCells[randomIndex];
        }
    }
}
