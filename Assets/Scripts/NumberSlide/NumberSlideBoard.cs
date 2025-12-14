using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.NumberSlide
{
    public class NumberSlideBoard : MonoBehaviour
    {
        [SerializeField] private NumberSlideGrid _grid;
        [SerializeField] private List<NumberSlideCellState> _tileStates;
        [SerializeField] private NumberSlideTile _tilePrefab;
        [SerializeField] private Transform _tilePool;

        [SerializeField] private float _moveDuration = 0.2f;

        private List<NumberSlideTile> _activeTiles = new List<NumberSlideTile>();
        private bool _isAnimating = false;

        public void StartNewGame()
        {
            ClearBoard();
            CreateTile();
            CreateTile();
        }

        public void CreateTile()
        {
            NumberSlideCell cell = _grid.GetRandomEmptyCell();
            if (cell == null) return;

            NumberSlideTile tile = PoolingManager.Spawn<NumberSlideTile>(_tilePrefab, _grid.transform.position, Quaternion.identity, _tilePool);
            tile.Initialize(_tileStates[0]);
            tile.SetTileCell(cell);
            tile.SnapToCell();

            tile.transform.localScale = Vector3.zero;
            tile.transform.DOScale(1f, 0.2f);

            _activeTiles.Add(tile);
        }

        public void ClearBoard()
        {
            foreach (NumberSlideRow row in _grid.Rows)
            {
                foreach (NumberSlideCell cell in row.Cells)
                {
                    cell.Tile = null;
                }
            }

            foreach (var tile in _activeTiles)
            {
                if (tile != null) Destroy(tile.gameObject);
            }

            _activeTiles.Clear();
            _isAnimating = false;
        }

        private void Update()
        {
            if (_isAnimating) return;

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveTile(Vector2Int.left);
            }
            else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveTile(Vector2Int.up);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveTile(Vector2Int.right);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveTile(Vector2Int.down);
            }
        }

        public void MoveTile(Vector2Int direction)
        {
            bool isMove = false;

            int startX = (direction.x > 0) ? _grid.GetWidth() - 1 : 0;
            int startY = (direction.y > 0) ? 0 : _grid.GetHeight() - 1;
            int stepX = (direction.x > 0) ? -1 : 1;
            int stepY = (direction.y > 0) ? 1 : -1;

            for (int x = startX; (stepX > 0) ? x < _grid.GetWidth() : x >= 0; x += stepX)
            {
                for (int y = startY; (stepY > 0) ? y < _grid.GetHeight() : y >= 0; y += stepY)
                {
                    NumberSlideCell cell = _grid.GetCell(x, y);
                    if (cell != null && cell.Occupied)
                    {
                        if (TryMoveTile(cell, direction))
                        {
                            isMove = true;
                        }
                    }
                }
            }

            if (isMove)
            {
                _isAnimating = true;

                foreach (var tile in _activeTiles)
                {
                    tile.MoveToCell(_moveDuration);
                }
                
                DOVirtual.DelayedCall(_moveDuration, () =>
                {
                    OnMoveCompleted();
                });
            }
        }

        private void OnMoveCompleted()
        {
            CreateTile();

            foreach (var tile in _activeTiles)
            {
                tile.ResetMerged();
            }

            _isAnimating = false;

            if (CheckEndGame())
            {
                NumberSlideGameManager.Instance.GameOver();
            }
        }

        public bool TryMoveTile(NumberSlideCell cell, Vector2Int direction)
        {
            NumberSlideCell newCell = null;
            NumberSlideCell adjacentCell = _grid.GetAdjacentCell(cell, direction);

            while (adjacentCell != null)
            {
                if (adjacentCell.Occupied)
                {
                    if (CanMerge(cell.Tile, adjacentCell.Tile))
                    {
                        MergeTiles(cell.Tile, adjacentCell.Tile);
                        return true;
                    }
                    break;
                }

                newCell = adjacentCell;
                adjacentCell = _grid.GetAdjacentCell(adjacentCell, direction);
            }

            if (newCell != null)
            {
                cell.Tile.SetTileCell(newCell);
                return true;
            }

            return false;
        }

        public bool CanMerge(NumberSlideTile tileA, NumberSlideTile tileB)
        {
            return tileA.State.number == tileB.State.number && !tileA.Merged && !tileB.Merged;
        }

        public void MergeTiles(NumberSlideTile tileA, NumberSlideTile tileB)
        {
            _activeTiles.Remove(tileA);
            tileA.Merge(tileB.Cell);

            int index = Mathf.Clamp(IndexOf(tileB.State) + 1, 0, _tileStates.Count - 1);
            NumberSlideCellState newState = _tileStates[index];

            tileB.Initialize(newState);
            tileB.Merged = true;

            DOVirtual.DelayedCall(_moveDuration, () => tileB.PlayMergeAnimation());

            NumberSlideGameManager.Instance.PlusScore(newState.number);
        }

        private int IndexOf(NumberSlideCellState state)
        {
            for (int i = 0; i < _tileStates.Count; i++)
            {
                if (state == _tileStates[i]) return i;
            }
            return -1;
        }

        public bool CheckEndGame()
        {
            if (_activeTiles.Count < _grid.GetSize()) return false;

            foreach (NumberSlideTile tile in _activeTiles)
            {
                Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
                foreach (var dir in dirs)
                {
                    NumberSlideCell neighbor = _grid.GetAdjacentCell(tile.Cell, dir);
                    if (neighbor != null && neighbor.Occupied && CanMerge(tile, neighbor.Tile))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}