using NguyenQuangMinh.MemoryCard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NguyenQuangMinh.SlidingPuzzle
{
    public class SlidingPuzzleGameManager : MonoBehaviour
    {
        [SerializeField] private int _sizeBoard;
        [SerializeField] private float _gapThickness, _timeShuffle;
        [SerializeField] private List<GameObject> _piecePrefab;
        [SerializeField] private List<Sprite> _finishImages;
        [SerializeField] private Transform _board;
        [SerializeField] private Image _finishImage;
        private GameObject _piecePuzzle;
        private List<GameObject> _pieces;
        private int _emptyLocation;
        private bool _shuffling = false;

        private void Start()
        {
            _pieces = new List<GameObject>();
            StartNewGame();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null)
                {
                    for (int i = 0; i < _pieces.Count; i++)
                    {
                        if (_pieces[i] == hit.transform)
                        {
                            if (SwapIfValid(i, -_sizeBoard, _sizeBoard)) { break; }
                            if (SwapIfValid(i, _sizeBoard, _sizeBoard)) { break; }
                            if (SwapIfValid(i, -1, 0)) { break; }
                            if (SwapIfValid(i, +1, _sizeBoard - 1)) { break; }
                        }
                    }
                    if (CheckCompletion())
                    {
                        Debug.Log("Win");
                    }
                }
            }
        }

        public void StartNewGame()
        {
            int index = Random.Range(0, _piecePrefab.Count);
            _piecePuzzle = _piecePrefab[index];
            CreateGamePieces(_sizeBoard, _gapThickness);
            StartShuffle();
            SetFinishImage(_finishImages[index]);
        }

        private void CreateGamePieces(int size, float gapThickness)
        {
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
                    GameObject piece = Instantiate(_piecePuzzle, _board);
                    _pieces.Add(piece);

                    piece.transform.localPosition = new Vector3((col * width) + offsetX, -(row * height) + offsetY, 0);
                    piece.transform.localScale = (new Vector3(width, height, 1) - new Vector3(gapThickness, gapThickness, 0));
                    piece.name = $"{(row * size) + col}";

                    if ((row == size - 1) && (col == size - 1))
                    {
                        _emptyLocation = (size * size) - 1;
                        piece.gameObject.SetActive(false);
                    }
                    else
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
                }
            }
        }

        private void SetFinishImage(Sprite sprite)
        {
            _finishImage.sprite = sprite;
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

        public void StartShuffle()
        {
            if (!_shuffling && CheckCompletion())
            {
                _shuffling = true;
                StartCoroutine(WaitShuffle(_timeShuffle));
            }
        }

        private bool SwapIfValid(int i, int offset, int colCheck)
        {
            if (((i % _sizeBoard) != colCheck) && ((i + offset) == _emptyLocation))
            {
                (_pieces[i], _pieces[i + offset]) = (_pieces[i + offset], _pieces[i]);
                (_pieces[i].transform.localPosition, _pieces[i + offset].transform.localPosition) = ((_pieces[i + offset].transform.localPosition, _pieces[i].transform.localPosition));
                _emptyLocation = i;
                return true;
            }
            return false;
        }
        private IEnumerator WaitShuffle(float duration)
        {
            yield return new WaitForSeconds(duration);
            Shuffle();
            _shuffling = false;
        }

        private void Shuffle()
        {
            int count = 0;
            int last = 0;
            while (count < (_sizeBoard * 15))
            {
                int rnd = Random.Range(0, _sizeBoard * _sizeBoard);
                if (rnd == last) { continue; }
                last = _emptyLocation;
                if (SwapIfValid(rnd, -_sizeBoard, _sizeBoard))
                {
                    count++;
                }
                else if (SwapIfValid(rnd, _sizeBoard, _sizeBoard))
                {
                    count++;
                }
                else if (SwapIfValid(rnd, -1, 0))
                {
                    count++;
                }
                else if (SwapIfValid(rnd, +1, _sizeBoard - 1))
                {
                    count++;
                }
            }
        }
    }
}
