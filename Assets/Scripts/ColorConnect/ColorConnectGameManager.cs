using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NguyenQuangMinh.ColorConnect
{
    public class ColorConnectGameManager : Singleton<ColorConnectGameManager>, IGameManager
    {
        [Header("Data Settings")]
        [SerializeField] private ColorPalette _activePalette;
        [SerializeField] private List<DataGrid> _levelData;
        [SerializeField] private GridLayoutGroup _layoutGroup;

        [Header("Scene References")]
        [SerializeField] private Tile _tilePrefab;
        [SerializeField] private Transform _gridContainer;
        [SerializeField] private SpriteRenderer _highlightClick;

        private Tile[,] _gridTiles;
        private bool _isDragging = false;
        private bool _isTransitioning = false;

        private TypeOfColor _currentDrawingColor = TypeOfColor.None;
        private Color _currentColor = Color.white;

        private List<Tile> _currentPath = new List<Tile>();
        private Dictionary<TypeOfColor, List<Tile>> _allPaths = new Dictionary<TypeOfColor, List<Tile>>();
        private DataGrid _currentLevelData;
        private int _currentLevelIndex = 0;
        private int _totalTilesCount = 0;
        private Camera _mainCamera;

        public void StartNewGame()
        {
            _mainCamera = Camera.main;
            if (_levelData != null && _levelData.Count > 0)
            {
                _currentLevelIndex = 0;
                LoadLevel(_levelData[0]);
            }
        }

        public void Restart()
        {
            LoadLevel(_levelData[_currentLevelIndex]);
        }

        public void LoadLevel(DataGrid data)
        {
            _currentLevelData = data;
            _allPaths.Clear();
            _isDragging = false;
            _isTransitioning = false;

            foreach (Transform child in _gridContainer)
            {
                PoolingManager.Despawn(child.gameObject);
            }

            _layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _layoutGroup.constraintCount = data.gridSize.x;
            _totalTilesCount = data.gridSize.x * data.gridSize.y;

            _gridTiles = new Tile[data.gridSize.x, data.gridSize.y];

            for (int x = 0; x < data.gridSize.x; x++)
            {
                for (int y = 0; y < data.gridSize.y; y++)
                {
                    Tile newTile = PoolingManager.Spawn(_tilePrefab, transform.position, Quaternion.identity, _gridContainer);
                    newTile.name = $"Tile_{x}_{y}";
                    newTile.Init(new Vector2Int(x, y));
                    _gridTiles[x, y] = newTile;
                }
            }

            if (_activePalette == null) return;

            foreach (DataNode node in data.dataNodes)
            {
                Color visualColor = _activePalette.GetColorByType(node.colorType);

                if (!_allPaths.ContainsKey(node.colorType))
                {
                    _allPaths[node.colorType] = new List<Tile>();
                }

                if (IsValidCoordinate(node.startPos))
                    _gridTiles[node.startPos.x, node.startPos.y].SetupDotColor(node.colorType, visualColor);

                if (IsValidCoordinate(node.endPos))
                    _gridTiles[node.endPos.x, node.endPos.y].SetupDotColor(node.colorType, visualColor);
            }
        }

        private bool IsValidCoordinate(Vector2Int pos)
        {
            if (_gridTiles == null) return false;
            return pos.x >= 0 && pos.x < _gridTiles.GetLength(0) &&
                   pos.y >= 0 && pos.y < _gridTiles.GetLength(1);
        }

        public void OnTilePointerDown(Tile tile)
        {
            if (_isTransitioning) return;

            if (tile.PathColorType != TypeOfColor.None)
            {
                _isDragging = true;
                _currentDrawingColor = tile.PathColorType;
                _currentColor = _activePalette.GetColorByType(_currentDrawingColor);

                if (!_allPaths.ContainsKey(_currentDrawingColor))
                {
                    _allPaths[_currentDrawingColor] = new List<Tile>(64);
                }

                _currentPath = _allPaths[_currentDrawingColor];

                if (tile.IsDot)
                {
                    ClearPathFromIndex(0);
                    AddToPath(tile);
                }
                else
                {
                    if (_currentPath.Contains(tile))
                    {
                        int index = _currentPath.IndexOf(tile);
                        ClearPathFromIndex(index + 1);
                    }
                }

                UpdatePathVisuals();
            }
        }

        public void OnTilePointerEnter(Tile tile)
        {
            if (!_isDragging || _isTransitioning) return;

            if (_currentPath.Count == 0) return;

            Tile lastTile = _currentPath[_currentPath.Count - 1];

            if (_currentPath.Count > 1 && lastTile.IsDot && _currentPath[0].IsDot) return;

            if (Vector2Int.Distance(tile.Coordinates, lastTile.Coordinates) != 1) return;

            if (_currentPath.Contains(tile))
            {
                int index = _currentPath.IndexOf(tile);
                ClearPathFromIndex(index + 1);
                UpdatePathVisuals();
                return;
            }
            if (tile.PathColorType != TypeOfColor.None)
            {
                if (tile.PathColorType != _currentDrawingColor) return;
            }

            AddToPath(tile);
            UpdatePathVisuals();
        }

        private void AddToPath(Tile tile)
        {
            if (!_currentPath.Contains(tile))
            {
                _currentPath.Add(tile);
                tile.SetPathColor(_currentDrawingColor, _currentColor);
            }
        }

        private void ClearPathFromIndex(int startIndex)
        {
            if (startIndex >= _currentPath.Count) return;

            for (int i = _currentPath.Count - 1; i >= startIndex; i--)
            {
                Tile t = _currentPath[i];
                t.ClearPath();
                _currentPath.RemoveAt(i);
            }
        }

        private void UpdatePathVisuals()
        {
            if (_currentPath.Count < 1) return;

            for (int i = 0; i < _currentPath.Count; i++)
            {
                Tile current = _currentPath[i];
                current.ResetEdges();
                if (i > 0)
                {
                    Tile prev = _currentPath[i - 1];
                    Vector2Int dir = prev.Coordinates - current.Coordinates;
                    current.EnableEdge(dir);
                }

                if (i < _currentPath.Count - 1)
                {
                    Tile next = _currentPath[i + 1];
                    Vector2Int dir = next.Coordinates - current.Coordinates;
                    current.EnableEdge(dir);
                }
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_currentDrawingColor == TypeOfColor.None) return;
                _highlightClick.gameObject.SetActive(true);
                _highlightClick.color = new Color(_currentColor.r, _currentColor.g, _currentColor.b, 0.35f);
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 10f;
                _highlightClick.transform.position = mousePos;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _highlightClick.gameObject.SetActive(false);
                _currentDrawingColor = TypeOfColor.None;
                if (_isDragging)
                {
                    _isDragging = false;
                    CheckWinCondition();
                }
            }
        }

        private void CheckWinCondition()
        {
            if (_currentLevelData == null) return;

            foreach (var node in _currentLevelData.dataNodes)
            {
                if (!_allPaths.ContainsKey(node.colorType)) return;

                List<Tile> path = _allPaths[node.colorType];
                if (path.Count < 2) return;

                Tile first = path[0];
                Tile last = path[path.Count - 1];

                bool connected = (first.IsDot && last.IsDot && first.Coordinates != last.Coordinates);
                if (!connected) return;
            }
            foreach (var tile in _gridTiles)
            {
                if (!tile.IsDot && tile.PathColorType == TypeOfColor.None)
                {
                    return;
                }
            }

            OnLevelCompleted();
        }

        private void OnLevelCompleted()
        {
            Debug.Log("VICTORY! BẠN ĐÃ CHIẾN THẮNG!");
            _isDragging = false;
            _isTransitioning = true;

            Invoke(nameof(LoadNextLevel), 1f);
        }

        private void LoadNextLevel()
        {
            _currentLevelIndex = (_currentLevelIndex + 1) % _levelData.Count;
            LoadLevel(_levelData[_currentLevelIndex]);
        }
    }
}