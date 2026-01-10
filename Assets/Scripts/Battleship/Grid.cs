using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.Battleship
{
    public class Grid : MonoBehaviour
    {
        [SerializeField] private List<Row> _rows;
        public List<Row> Rows => _rows;
        [SerializeField] private List<Cell> _cells;
        public List<Cell> Cells => _cells;

        private int size => _cells.Count;
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

        public void Init()
        {
            for (int y = 0; y < _rows.Count; y++)
            {
                for (int x = 0; x < _rows[y].Cells.Count; x++)
                {
                    _rows[y].Cells[x].Init(x, y);
                    _cells.Add(_rows[y].Cells[x]);
                }
            }
        }

        public Cell GetCell(Vector2Int coordinates)
        {
            return GetCell(coordinates.x, coordinates.y);
        }

        public Cell GetCell(int x, int y)
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

        public Vector2Int GetCoordFromWorldPos(Vector3 pos)
        {
            foreach (var row in _rows)
            {
                foreach (var cell in row.Cells)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(cell.GetComponent<RectTransform>(), pos, Camera.main))
                    {
                        return cell.Coordinates;
                    }
                }
            }
            return new Vector2Int(-1, -1);
        }

        public bool IsValidCoord(Vector2Int coord)
        {
            return coord.x >= 0 && coord.x < width && coord.y >= 0 && coord.y < height;
        }
    }

}