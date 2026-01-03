using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace NguyenQuangMinh.Tetris
{
    public enum Tetromino
    {
        I,
        O,
        T,
        J,
        L,
        S,
        Z,
        Dot
    }

    [System.Serializable]
    public struct TetrominoData
    {
        public Tetromino tetromino;
        public Tile tile;
        private Vector2Int[] _cells;
        public Vector2Int[] Cells => _cells;
        private Vector2Int[,] _wallKicks;
        public Vector2Int[,] WallKicks => _wallKicks;

        public void Init()
        {
            if (Data.Cells == null || Data.WallKicks == null)
            {
                Debug.LogError("Data.Cells or Data.WallKicks is not initialized.");
                return;
            }

            if (_cells == null)
                _cells = Data.Cells[tetromino];
            if (_wallKicks == null)
                _wallKicks = Data.WallKicks[tetromino];

            if (_cells == null)
            {
                Debug.LogError($"Cells for tetromino {tetromino} is not found in Data.Cells.");
            }
            if (_wallKicks == null)
            {
                Debug.LogError($"WallKicks for tetromino {tetromino} is not found in Data.WallKicks.");
            }
        }
    }
}
