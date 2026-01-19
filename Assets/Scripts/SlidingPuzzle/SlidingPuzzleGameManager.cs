using DG.Tweening;
using NguyenQuangMinh.MemoryCard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NguyenQuangMinh.SlidingPuzzle
{
    public class SlidingPuzzleGameManager : Singleton<SlidingPuzzleGameManager>, IGameManager
    {
        [Header("Settings")]
        [SerializeField] private int _sizeBoard = 3;
        [SerializeField] private float _gapThickness = 0.05f;
        [SerializeField] private float _moveDuration = 0.2f;
        [SerializeField] private int _shuffleSteps = 50;

        [Header("References")]
        [SerializeField] private List<GameObject> _piecePrefab;
        [SerializeField] private List<Sprite> _finishImages;
        [SerializeField] private Transform _board;
        [SerializeField] private Image _finishImage;

        [Header("Effects")]
        [SerializeField] private ParticleSystem _winParticle;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _moveSound;
        [SerializeField] private AudioClip _winSound;

        private List<GameObject> _pieces;
        private int _emptyLocation;
        private bool _isMoving = false;
        private bool _isGameActive = false;
        private int _gridSize;

        public void StartNewGame()
        {
            _pieces = new List<GameObject>();
            foreach (Transform child in _board)
            {
                Destroy(child.gameObject);
            }
            _pieces.Clear();

            int index = Random.Range(0, _piecePrefab.Count);
            GameObject prefab = _piecePrefab[index];
            SetFinishImage(_finishImages[index]);

            CreateGamePieces(prefab, _sizeBoard, _gapThickness);

            StartCoroutine(ShuffleRoutine());
        }

        public void Restart()
        {
            StartNewGame();
        }

        private void Update()
        {
            if (!_isGameActive || _isMoving || MainGameManager.Instance.GameState != GameState.InGame) return;

            if (Input.GetMouseButtonDown(0))
            {
                HandleInput();
            }
        }

        private void HandleInput()
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                AudioManager.Instance.PlaySlidingTileMoveSound();
                int clickedIndex = _pieces.IndexOf(hit.transform.gameObject);
                if (clickedIndex != -1)
                {
                    TryMove(clickedIndex);
                }
            }
        }


        private void CreateGamePieces(GameObject prefab, int size, float gapThickness)
        {
            _gridSize = size * size;
            float boardScaleX = _board.localScale.x;
            float boardScaleY = _board.localScale.y;

            float width = boardScaleX / size;
            float height = boardScaleY / size;

            float offsetX = -boardScaleX / 2 + width / 2;
            float offsetY = boardScaleY / 2 - height / 2;

            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    GameObject piece = Instantiate(prefab, _board);
                    _pieces.Add(piece);

                    Vector3 targetPos = new Vector3((col * width) + offsetX, -(row * height) + offsetY, 0);
                    piece.transform.localPosition = targetPos;
                    piece.transform.localScale = (new Vector3(width, height, 1) - new Vector3(gapThickness, gapThickness, 0));

                    int currentIndex = (row * size) + col;
                    piece.name = $"{currentIndex}";

                    SetupUV(piece, size, row, col, gapThickness);

                    if (currentIndex == _gridSize - 1)
                    {
                        _emptyLocation = currentIndex;
                        piece.gameObject.SetActive(false);
                    }

                    if (piece.GetComponent<Collider2D>() == null)
                        piece.AddComponent<BoxCollider2D>();
                }
            }
        }

        private void SetupUV(GameObject piece, int size, int row, int col, float gapThickness)
        {
            float gap = gapThickness / 2;
            Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
            Vector2[] uv = new Vector2[4];

            uv[0] = new Vector2((1f / size * col) + gap, 1 - ((1f / size * (row + 1)) - gap));
            uv[1] = new Vector2((1f / size * (col + 1)) - gap, 1 - ((1f / size * (row + 1)) - gap));
            uv[2] = new Vector2((1f / size * col) + gap, 1 - ((1f / size * row) + gap));
            uv[3] = new Vector2((1f / size * (col + 1)) - gap, 1 - ((1f / size * row) + gap));
            mesh.uv = uv;
        }

        private void TryMove(int index)
        {
            if (IsValidMove(index, _emptyLocation))
            {
                StartCoroutine(SwapPieceSmoothly(index, _emptyLocation));
            }
        }

        private bool IsValidMove(int targetIndex, int emptyIndex)
        {
            if (Mathf.Abs(targetIndex - emptyIndex) == 1)
            {
                int min = Mathf.Min(targetIndex, emptyIndex);
                if ((min + 1) % _sizeBoard == 0) return false;
                return true;
            }

            if (Mathf.Abs(targetIndex - emptyIndex) == _sizeBoard)
            {
                return true;
            }

            return false;
        }

        private IEnumerator SwapPieceSmoothly(int pieceIndex, int emptyIndex)
        {
            _isMoving = true;

            if (_audioSource && _moveSound) _audioSource.PlayOneShot(_moveSound);

            GameObject pieceObj = _pieces[pieceIndex];
            GameObject emptyObj = _pieces[emptyIndex];

            Vector3 startPos = pieceObj.transform.localPosition;
            Vector3 endPos = emptyObj.transform.localPosition;

            float elapsed = 0;
            while (elapsed < _moveDuration)
            {
                pieceObj.transform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / _moveDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            pieceObj.transform.localPosition = endPos;
            emptyObj.transform.localPosition = startPos;

            SwapData(pieceIndex, emptyIndex);

            _emptyLocation = pieceIndex;

            _isMoving = false;

            if (_isGameActive && CheckCompletion())
            {
                OnWin();
            }
        }

        private void SwapInstant(int pieceIndex, int emptyIndex)
        {
            GameObject pieceObj = _pieces[pieceIndex];
            GameObject emptyObj = _pieces[emptyIndex];

            Vector3 tempPos = pieceObj.transform.localPosition;
            pieceObj.transform.localPosition = emptyObj.transform.localPosition;
            emptyObj.transform.localPosition = tempPos;

            SwapData(pieceIndex, emptyIndex);
            _emptyLocation = pieceIndex;
        }

        private void SwapData(int i, int j)
        {
            GameObject temp = _pieces[i];
            _pieces[i] = _pieces[j];
            _pieces[j] = temp;
        }

        private IEnumerator ShuffleRoutine()
        {
            _isMoving = true;
            _isGameActive = false;
            int lastMove = -1;

            for (int i = 0; i < _shuffleSteps; i++)
            {
                List<int> neighbors = GetValidNeighbors(_emptyLocation);

                if (lastMove != -1 && neighbors.Contains(lastMove))
                {
                    neighbors.Remove(lastMove);
                }

                if (neighbors.Count > 0)
                {
                    int rndIndex = neighbors[Random.Range(0, neighbors.Count)];

                    lastMove = _emptyLocation;

                    SwapInstant(rndIndex, _emptyLocation);
                }

                yield return new WaitForSeconds(0.02f);
            }

            _isMoving = false;
            _isGameActive = true;
        }

        private List<int> GetValidNeighbors(int emptyIndex)
        {
            List<int> neighbors = new List<int>();

            if (emptyIndex - _sizeBoard >= 0) neighbors.Add(emptyIndex - _sizeBoard);
            if (emptyIndex + _sizeBoard < _gridSize) neighbors.Add(emptyIndex + _sizeBoard);
            if (emptyIndex % _sizeBoard != 0) neighbors.Add(emptyIndex - 1);
            if ((emptyIndex + 1) % _sizeBoard != 0) neighbors.Add(emptyIndex + 1);

            return neighbors;
        }

        private bool CheckCompletion()
        {
            for (int i = 0; i < _pieces.Count; i++)
            {
                if (_pieces[i].name != $"{i}")
                {
                    return false;
                }
            }
            return true;
        }

        private void OnWin()
        {
            Debug.Log("WINNER!");
            _isGameActive = false;

            if (_pieces != null && _emptyLocation < _pieces.Count)
            {
                _pieces[_emptyLocation].SetActive(true);
            }

            DOVirtual.DelayedCall(1f, () =>
            {
                MainGameManager.Instance.ShowWinUI(false);
            });
        }

        private void SetFinishImage(Sprite sprite)
        {
            if (_finishImage) _finishImage.sprite = sprite;
        }
    }
}