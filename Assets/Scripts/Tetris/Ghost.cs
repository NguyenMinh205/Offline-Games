using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace NguyenQuangMinh.Tetris
{
    public class Ghost : MonoBehaviour
    {
        [SerializeField] private Tile _tileGhost;
        [SerializeField] private Tilemap _tilemapGhost;
        public Tilemap TilemapGhost => _tilemapGhost;
        [SerializeField] private Piece _piece;
        private Vector3Int[] _cells;
        private Vector3Int _position;       

        private void LateUpdate()
        {
            if (_piece.Cells == null) return;

            Clear();
            Copy();
            Drop();
            Set();
        }

        private void Clear()
        {
            if (_cells == null) return;
            for (int i = 0; i < _cells.Length; i++)
            {
                Vector3Int tilePosition = _cells[i] + _position;
                _tilemapGhost.SetTile(tilePosition, null);
            }
        }

        private void Copy()
        {
            if (_piece.Cells == null) return;
            _cells = new Vector3Int[_piece.Cells.Length];
            for (int i = 0; i < _cells  .Length; i++)
            {
                _cells[i] = _piece.Cells[i];
            }
        }

        private void Drop()
        {
            Vector3Int pos = _piece.Position;
            int currentY = pos.y;
            int bottom = BoardManager.Instance.BoardBounds.yMin;

            BoardManager.Instance.ClearPiece(_piece);

            for (int y = currentY; y >= bottom; y--)
            {
                pos.y = y;
                if (BoardManager.Instance.IsValidPosition(_piece, pos))
                {
                    _position = pos;
                }
                else
                {
                    break;
                }
            }

            BoardManager.Instance.SetPiece(_piece);
        }

        private void Set()
        {
            for (int i = 0; i < _piece.Cells.Length; i++)
            {
                Vector3Int tilePosition = _cells[i] + _position;
                _tilemapGhost.SetTile(tilePosition, _tileGhost);
            }
        }
    }
}
