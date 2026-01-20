using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace NguyenQuangMinh.Tetris
{
    public class BoardManager : Singleton<BoardManager>
    {
        [SerializeField] private Tilemap _tilemap;
        public Tilemap Tilemap => _tilemap;

        [SerializeField] private Piece _piece;
        public Piece Piece => _piece;

        [SerializeField] private List<TetrominoData> _tetrominoDatas;
        public List<TetrominoData> TetrominoDatas => _tetrominoDatas;

        [SerializeField] private Vector3Int _spawnPosition = new Vector3Int(5, 20, 0);
        public Vector3Int SpawnPosition => _spawnPosition;

        [SerializeField] private Vector2Int _boardSize = new Vector2Int(12, 20);
        public Vector2Int BoardSize => _boardSize;

        public RectInt BoardBounds
        {
            get
            {
                Vector2Int pos = new Vector2Int(-_boardSize.x / 2, -_boardSize.y / 2 - 2);
                return new RectInt(pos, _boardSize);
            }
        }

        private void OnEnable()
        {
            for (int i = 0; i < _tetrominoDatas.Count; i++)
            {
                TetrominoData data = _tetrominoDatas[i];
                data.Init();
                _tetrominoDatas[i] = data;
            }
        }

        public void ResetBoard()
        {
            _tilemap.ClearAllTiles();
            if (_piece != null)
            {
                ClearPiece(_piece);
            }
        }

        public void StartNewGame()
        {
            ResetBoard();
            DOVirtual.DelayedCall(0.5f, () =>
            {
                SpawnPiece();
            });
        }

        public void SpawnPiece()
        {
            int rnd = Random.Range(0, _tetrominoDatas.Count);
            TetrominoData data = _tetrominoDatas[rnd];

            _piece.Init(_spawnPosition, data);
            if (IsValidPosition(_piece, _spawnPosition))
            {
                SetPiece(_piece);
            }
            else
            {
                TetrisGameManager.Instance.GameOver();
            }
        }

        public void SetPiece(Piece piece)
        {
            if (piece == null || piece.Cells == null) return;

            for (int i = 0; i < piece.Cells.Length; i++)
            {
                Vector3Int cell = piece.Cells[i];
                Vector3Int tilePosition = (Vector3Int)cell + piece.Position;
                _tilemap.SetTile(tilePosition, piece.TetrominoData.tile);
            }
        }

        public void ClearPiece(Piece piece)
        {
            if (piece == null || piece.Cells == null) return;
            for (int i = 0; i < piece.Cells.Length; i++)
            {
                Vector3Int cell = piece.Cells[i];
                Vector3Int tilePosition = (Vector3Int)cell + piece.Position;
                _tilemap.SetTile(tilePosition, null);
            }
        }

        public bool IsValidPosition(Piece piece, Vector3Int position)
        {
            if (piece == null || piece.Cells == null) return false;
            RectInt boards = BoardBounds;
            for (int i = 0; i < piece.Cells.Length; i++)
            {
                Vector3Int cell = piece.Cells[i];
                Vector3Int tilePosition = cell + position;

                if (!boards.Contains((Vector2Int)tilePosition)) return false;
                if (_tilemap.HasTile(tilePosition)) return false;
            }
            return true;
        }
        public void ClearLines()
        {
            RectInt bound = BoardBounds;
            int row = bound.yMin;
            int linesCleared = 0;

            while (row < bound.yMax)
            {
                if (IsLineFull(row))
                {
                    ClearLine(row);
                    linesCleared++;
                }
                else
                {
                    row++;
                }
            }

            if (linesCleared > 0)
            {
                TetrisGameManager.Instance.AddScore(linesCleared);
                AudioManager.Instance.PlayTetrisClearRowSound();
            }
        }

        public bool IsLineFull(int row)
        {
            RectInt bound = BoardBounds;
            for (int col = bound.xMin; col < bound.xMax; col++)
            {
                Vector3Int pos = new Vector3Int(col, row, 0);
                if (!_tilemap.HasTile(pos)) return false;
            }
            return true;
        }

        public void ClearLine(int row)
        {
            RectInt bound = BoardBounds;
            for (int col = bound.xMin; col < bound.xMax; col++)
            {
                Vector3Int pos = new Vector3Int(col, row, 0);
                _tilemap.SetTile(pos, null);
            }
            for (int r = row + 1; r < bound.yMax; r++)
            {
                for (int col = bound.xMin; col < bound.xMax; col++)
                {
                    Vector3Int pos = new Vector3Int(col, r, 0);
                    TileBase tile = _tilemap.GetTile(pos);
                    _tilemap.SetTile(new Vector3Int(col, r - 1, 0), tile);
                }
            }
            for (int col = bound.xMin; col < bound.xMax; col++)
            {
                Vector3Int pos = new Vector3Int(col, bound.yMax - 1, 0);
                _tilemap.SetTile(pos, null);
            }
        }
    }
}