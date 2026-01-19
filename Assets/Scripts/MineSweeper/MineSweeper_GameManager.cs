using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NguyenQuangMinh.MineSweeper
{
    public class MineSweeper_GameManager : Singleton<MineSweeper_GameManager>, IGameManager
    {
        [SerializeField] private Transform _tilePrefab;
        [SerializeField] private Transform _tileStore;
        [SerializeField] private Button _flagButton;
        [SerializeField] private Button _clickedButton;
        [SerializeField] private TextMeshProUGUI _mineCountText;
        [SerializeField] private Color _inactiveColor;
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private int _numMines;

        private List<Tile_MineSweeper> _tiles = new List<Tile_MineSweeper>();
        private List<int> minePos = new List<int>();

        private bool _isFlagMode;
        public bool IsFlagMode => _isFlagMode;

        private readonly float _tileSize = 0.6f;

        [Header("Game State")]
        private int revealedCount = 0;
        private bool isGameOver = false;
        private int remainingFlags = 0;

        public void StartNewGame()
        {
            CloseFlagMode();
            isGameOver = false;
            revealedCount = 0;

            foreach (var tile in _tiles)
            {
                PoolingManager.Despawn(tile.gameObject);
            }
            _tiles.Clear();
            CreateGameBoard(_width, _height, _numMines);
        }

        public void Restart()
        {
            StartNewGame();
        }

        public void OpenFlagMode()
        {
            _isFlagMode = true;
            _flagButton.image.color = Color.white;
            _clickedButton.image.color = _inactiveColor;
        }

        public void CloseFlagMode()
        {
            _isFlagMode = false;
            _flagButton.image.color = _inactiveColor;
            _clickedButton.image.color = Color.white;
        }

        public void CreateGameBoard(int _width, int _height, int _numMines)
        {
            this._width = _width;
            this._height = _height;
            this._numMines = _numMines;
            remainingFlags = _numMines;
            _mineCountText.text = remainingFlags.ToString();

            for (int row = 0; row < _height; row++)
            {
                for (int col = 0; col < _width; col++)
                {
                    Transform tileTransform = PoolingManager.Spawn(_tilePrefab, this.transform.position, Quaternion.identity, _tileStore);
                    float xIndex = col - ((_width - 1) / 2.0f);
                    float yIndex = row - ((_height - 1) / 2.0f);
                    tileTransform.localPosition = new Vector2(xIndex * _tileSize, yIndex * _tileSize);
                    Tile_MineSweeper tile = tileTransform.GetComponent<Tile_MineSweeper>();
                    tile.Init();
                    _tiles.Add(tile);
                }
            }
            GenerateMines();
            SetupTile();
        }

        public void GenerateMines()
        {
            minePos.Clear();
            minePos = Enumerable.Range(0, _tiles.Count)
                                    .OrderBy(x => Random.Range(0.0f, 1.0f))
                                    .Take(_numMines)
                                    .ToList();
        }

        public void SetupTile()
        {
            for (int i = 0; i < minePos.Count; i++)
            {
                int pos = minePos[i];
                _tiles[pos].IsMine = true;
            }

            for (int i = 0; i < _tiles.Count; i++)
            {
                _tiles[i].MineCount = HowManyMines(i);
            }
        }

        public int HowManyMines(int pos)
        {
            int count = 0;
            List<int> neighborTiles = GetNeighborTiles(pos);

            foreach (int n in neighborTiles)
            {
                if (_tiles[n].IsMine)
                    count++;
            }

            return count;
        }

        public List<int> GetNeighborTiles(int pos)
        {
            List<int> neighbors = new List<int>();

            int row = pos / _width;
            int col = pos % _width;

            int[] dRow = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dCol = { -1, 0, 1, -1, 1, -1, 0, 1 };

            for (int i = 0; i < 8; i++)
            {
                int newRow = row + dRow[i];
                int newCol = col + dCol[i];

                if (newRow >= 0 && newRow < _height && newCol >= 0 && newCol < _width)
                {
                    int neighborIndex = newRow * _width + newCol;
                    neighbors.Add(neighborIndex);
                }
            }

            return neighbors;
        }

        public void HandleTileClick(Tile_MineSweeper tile)
        {
            if (isGameOver) return;
            if (tile == null) return;

            if (_isFlagMode)
            {
                AudioManager.Instance.PlayMinesweeperTileClick();
                bool nowFlagged = !tile.IsFlagged;
                if (nowFlagged)
                {
                    if (!tile.IsRevealed)
                    {
                        tile.SetFlagSpriteDirect();
                        remainingFlags--;
                    }
                }
                else
                {
                    if (!tile.IsRevealed)
                    {
                        tile.SetUnclickedSpriteDirect();
                        remainingFlags++;
                    }
                }
                _mineCountText.text = Mathf.Max(0, remainingFlags).ToString();
                CheckWinCondition();
                return;
            }

            if (tile.IsRevealed || tile.IsFlagged) return;

            tile.RevealTile();
            revealedCount++;

            if (tile.MineCount == 0)
            {
                int index = _tiles.IndexOf(tile);
                if (index < 0) return;

                RevealEmptyRegionBFS(index);
            }
            AudioManager.Instance.PlayMinesweeperTileExpand();
            CheckWinCondition();
        }

        public void RevealEmptyRegionBFS(int startIndex)
        {
            if (startIndex < 0 || startIndex >= _tiles.Count) return;

            Queue<int> q = new Queue<int>();
            HashSet<int> visited = new HashSet<int>();

            visited.Add(startIndex);
            q.Enqueue(startIndex);

            while (q.Count > 0)
            {
                int cur = q.Dequeue();
                Tile_MineSweeper tile = _tiles[cur];

                if (!tile.IsRevealed && !tile.IsFlagged)
                {
                    tile.RevealTile();
                    revealedCount++;
                }

                if (tile.MineCount == 0 && tile.IsMine != true)
                {
                    foreach (int nei in GetNeighborTiles(cur))
                    {
                        if (visited.Contains(nei)) continue;
                        Tile_MineSweeper nTile = _tiles[nei];
                        if (!nTile.IsRevealed && !nTile.IsFlagged)
                        {
                            visited.Add(nei);
                            q.Enqueue(nei);
                        }
                    }
                }
            }
        }

        public void GameOver()
        {
            isGameOver = true;
            for (int i = 0; i < _tiles.Count; i++)
            {
                Tile_MineSweeper t = _tiles[i];

                if (t.IsMine && !t.IsFlagged && !t.IsRevealed)
                {
                    t.ShowMineSprite();
                }
            }
            AudioManager.Instance.PlayMinesweeperExplosion();

            DOVirtual.DelayedCall(1f, () =>
            {
                MainGameManager.Instance.ShowLoseUI();
            });
        }

        private void CheckWinCondition()
        {
            int totalSafe = _tiles.Count - minePos.Count;
            if (revealedCount >= totalSafe && !isGameOver)
            {
                isGameOver = true;
                for (int i = 0; i < _tiles.Count; i++)
                {
                    var t = _tiles[i];
                    if (t.IsMine && !t.IsFlagged)
                    {
                        t.ShowMineSprite();
                    }
                }

                DOVirtual.DelayedCall(1f, () =>
                {
                    MainGameManager.Instance.ShowWinUI(false);
                });
            }
        }
    }
}
