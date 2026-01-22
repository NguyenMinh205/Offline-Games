using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.Tetris
{
    public class Piece : MonoBehaviour
    {
        private Vector3Int _position;
        public Vector3Int Position => _position;
        private TetrominoData _data;
        public TetrominoData TetrominoData => _data;
        private Vector3Int[] _cells;
        public Vector3Int[] Cells => _cells;
        private int _rotationIndex;
        public int RotationIndex => _rotationIndex;

        [SerializeField] private float stepDelay = 1f;
        private float _stepTime;

        public void Init(Vector3Int position, TetrominoData data)
        {
            _position = position;
            _data = data;

            _stepTime = 0f;
            _rotationIndex = 0;

            if (_cells == null || _cells.Length != data.Cells.Length)
            {
                _cells = new Vector3Int[data.Cells.Length];
            }
            for (int i = 0; i < data.Cells.Length; i++)
            {
                _cells[i] = (Vector3Int)data.Cells[i];
            }
        }

        private void Update()
        {
            if (_cells == null || TetrisGameManager.Instance.IsGameOver) return;

            BoardManager.Instance.ClearPiece(this);

            _stepTime += Time.deltaTime;

            if (_stepTime >= stepDelay)
            {
                Step();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                Move(Vector2Int.left);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                Move(Vector2Int.right);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                Rotate(1);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                HardDrop();
            }

            BoardManager.Instance.SetPiece(this);
        }

        private void Step()
        {
            _stepTime = 0f; 
            if (!Move(Vector2Int.down))
            {
                Lock();
            }
        }

        private void HardDrop()
        {
            while (Move(Vector2Int.down))
            {
                continue;
            }
            Lock();
        }

        public bool Move(Vector2Int translation)
        {
            Vector3Int newPos = _position;
            newPos.x += translation.x;
            newPos.y += translation.y;

            if (BoardManager.Instance.IsValidPosition(this, newPos))
            {
                _position = newPos;
                return true;
            }
            return false;
        }

        public void Rotate(int direction)
        {
            int previousRotationIndex = _rotationIndex; 
            _rotationIndex = Wrap(_rotationIndex + direction, 0, 4);

            ApplyRotation(direction);

            if (!IsWallKick(_rotationIndex, direction))
            {
                _rotationIndex = previousRotationIndex;
                ApplyRotation(-direction);
            }
        }

        public void ApplyRotation(int direction)
        {

            for (int i = 0; i < _cells.Length; i++)
            {   
                Vector3 cell = _cells[i];
                int x, y;

                switch (_data.tetromino)
                {

                    case Tetromino.I:
                    case Tetromino.O:
                        cell.x -= 0.5f;
                        cell.y -= 0.5f;
                        x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                        y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                        break;
                    default:
                        x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                        y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));

                        break;
                }

                _cells[i] = new Vector3Int(x, y, 0);
            }
        }

        private bool IsWallKick(int rotationIndex, int rotationDicrection)
        {
            int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDicrection);

            for (int i = 0; i < _data.WallKicks.GetLength(1); i++)
            {
                Vector2Int translation = _data.WallKicks[wallKickIndex, i];
                if (Move(translation))
                {
                    return true;
                }
            }   
            return false;
        }

        private int GetWallKickIndex(int rotationIndex, int rotationDicrection)
        {
            int wallKickIndex = rotationIndex * 2;

            if (rotationDicrection < 0)
            {
                wallKickIndex--;
            }

            return Wrap(wallKickIndex, 0, _data.WallKicks.GetLength(0));
        }

        private int Wrap(int input, int min, int max)
        {
            if (input < min)
            {
                return max - (min - input) % (max - min);
            }
            else
            {
                return min + (input - min) % (max - min);
            }
        }

        private void Lock()
        {
            AudioManager.Instance.PlayTetrisDropSound();
            BoardManager.Instance.SetPiece(this);
            BoardManager.Instance.ClearLines();
            BoardManager.Instance.SpawnPiece();
        }

        public void ResetData()
        {
            _cells = null;
            _stepTime = 0f;
            _rotationIndex = 0;
        }
    }
}