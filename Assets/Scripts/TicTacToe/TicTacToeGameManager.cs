using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using TMPro;
using UnityEditor.MPE;

namespace NguyenQuangMinh.TicTacToe
{
    public class TicTacToeGameManager : Singleton<TicTacToeGameManager>, IGameManager
    {
        [Header("References")]
        [SerializeField] private TicTacToeGridManager _gridManager;
        [SerializeField] private WinLineManager _winLineManager;

        [Header("UI")]
        [SerializeField] private Image _background;
        [SerializeField] private TextMeshProUGUI _turnText;
        [SerializeField] private Color _OTurnBG;
        [SerializeField] private Color _XTurnBG;

        [Header("Game Settings")]
        [SerializeField] private float _botDelay = 0.5f;

        [Range(0f, 1f)]
        [SerializeField] private float _botDifficulty = 0.7f;

        private static readonly int[,] _winCombos = new int[,] {
            {0, 1, 2}, {3, 4, 5}, {6, 7, 8},
            {0, 3, 6}, {1, 4, 7}, {2, 5, 8},
            {0, 4, 8}, {2, 4, 6}
        };

        private static readonly WinLine[] _winLineTypes = new WinLine[] {
            WinLine.HorizontalTop, WinLine.HorizontalMiddle, WinLine.HorizontalBottom,
            WinLine.VerticalLeft, WinLine.VerticalMiddle, WinLine.VerticalRight,
            WinLine.DiagonalDown, WinLine.DiagonalUp
        };

        private CellState _playerState;
        private CellState _botState;
        private Color _playerTurnBG;
        private Color _botTurnBG;
        private Turn _currentTurn;
        private bool _isGameActive = false;

        private Coroutine _botCoroutine;
        private Coroutine _drawCoroutine;

        public void StartNewGame()
        {
            int firstTurn = Random.Range(0, 2);
            _currentTurn = (firstTurn == 0) ? Turn.Player : Turn.Bot;

            if (_turnText) _turnText.gameObject.SetActive(true);
            SetupGame();
        }

        public void Restart()
        {
            if (_currentTurn == Turn.Player)
            {
                Debug.Log("Restart: Bot starts first");
                _currentTurn = Turn.Bot;
            }
            else
            {
                Debug.Log("Restart: Player starts first");
                _currentTurn = Turn.Player;
            }
            SetupGame();
        }

        public void SetupGame()
        {
            StopAllGameCoroutines();

            _gridManager.SetupGrid();

            if (_winLineManager != null)
                _winLineManager.SetWinLine(WinLine.None);

            if (_currentTurn == Turn.Player)
            {
                _playerState = CellState.X;
                _playerTurnBG = _XTurnBG;
                _botState = CellState.O;
                _botTurnBG = _OTurnBG;
            }
            else
            {
                _playerState = CellState.O;
                _playerTurnBG = _OTurnBG;
                _botState = CellState.X;
                _botTurnBG = _XTurnBG;
            }

            _isGameActive = true;

            if (_currentTurn == Turn.Bot)
            {
                if (_background) _background.color = _botTurnBG;
                if (_turnText) _turnText.text = "Bot's Turn";
                _botCoroutine = StartCoroutine(BotTurnRoutine());
            }
            else
            {
                if (_background) _background.color = _playerTurnBG;
                if (_turnText) _turnText.text = "Your Turn";
            }
        }

        public void CellClicked(int cellID)
        {
            if (!_isGameActive || _currentTurn != Turn.Player) return;
            var cell = _gridManager.GetCellControllerAt(cellID);
            if (cell == null || cell.CurrentState != CellState.Empty) return;

            ProcessTurn(Turn.Player, cellID);
        }

        private void ProcessTurn(Turn turn, int selectedCell)
        {
            if (turn == Turn.Player)
            {
                AudioManager.Instance.PlayTicTacToePlayerClick();
            }
            else
            {
                AudioManager.Instance.PlayTicTacToeEnemyClick();
            }
            CellState state = (turn == Turn.Player) ? _playerState : _botState;
            _gridManager.SetSpecificCell(state, selectedCell);

            if (CheckWin())
            {
                EndGame(turn);
                return;
            }

            if (_gridManager.CheckFullGrid())
            {
                _drawCoroutine = StartCoroutine(HandleDrawRoutine());
                return;
            }

            ChangeTurn();
        }

        private IEnumerator HandleDrawRoutine()
        {
            _isGameActive = false;
            if (_turnText) _turnText.text = "Draw!";

            yield return new WaitForSeconds(1f);

            Restart();
        }

        private void ChangeTurn()
        {
            _currentTurn = (_currentTurn == Turn.Player) ? Turn.Bot : Turn.Player;

            if (_currentTurn == Turn.Bot)
            {
                if (_background) _background.color = _botTurnBG;
                if (_turnText) _turnText.text = "Bot's Turn";
                _botCoroutine = StartCoroutine(BotTurnRoutine());
            }
            else
            {
                if (_background) _background.color = _playerTurnBG;
                if (_turnText) _turnText.text = "Your Turn";
            }
        }

        private IEnumerator BotTurnRoutine()
        {
            yield return new WaitForSeconds(_botDelay);

            if (!_isGameActive) yield break;

            int move = GetBotMove();
            if (move != -1)
            {
                ProcessTurn(Turn.Bot, move);
            }
        }

        private void StopAllGameCoroutines()
        {
            if (_botCoroutine != null) StopCoroutine(_botCoroutine);
            if (_drawCoroutine != null) StopCoroutine(_drawCoroutine);
            _botCoroutine = null;
            _drawCoroutine = null;
        }

        private int GetBotMove()
        {
            List<int> availableMoves = new List<int>();
            var grid = _gridManager.Grid;
            for (int i = 0; i < grid.Count; i++)
            {
                if (grid[i].CurrentState == CellState.Empty) availableMoves.Add(i);
            }

            if (availableMoves.Count == 0) return -1;

            float roll = Random.value;

            if (roll < _botDifficulty)
            {
                CellState[] virtualBoard = new CellState[9];
                for (int i = 0; i < 9; i++)
                {
                    virtualBoard[i] = grid[i].CurrentState;
                }

                int bestScore = int.MinValue;
                int bestMove = availableMoves[0];

                foreach (int index in availableMoves)
                {
                    virtualBoard[index] = _botState;
                    int score = Minimax(virtualBoard, 0, false);
                    virtualBoard[index] = CellState.Empty;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = index;
                    }
                }
                return bestMove;
            }
            else
            {
                return availableMoves[Random.Range(0, availableMoves.Count)];
            }
        }

        private int Minimax(CellState[] board, int depth, bool isMaximizing)
        {
            if (IsWinnerVirtual(board, _botState)) return 10 - depth;
            if (IsWinnerVirtual(board, _playerState)) return depth - 10;

            if (!IsAnyCellEmpty(board)) return 0;

            if (isMaximizing)
            {
                int bestScore = int.MinValue;
                for (int i = 0; i < 9; i++)
                {
                    if (board[i] == CellState.Empty)
                    {
                        board[i] = _botState;
                        int score = Minimax(board, depth + 1, false);
                        board[i] = CellState.Empty;
                        bestScore = Mathf.Max(score, bestScore);
                    }
                }
                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;
                for (int i = 0; i < 9; i++)
                {
                    if (board[i] == CellState.Empty)
                    {
                        board[i] = _playerState;
                        int score = Minimax(board, depth + 1, true);
                        board[i] = CellState.Empty;
                        bestScore = Mathf.Min(score, bestScore);
                    }
                }
                return bestScore;
            }
        }

        private bool IsAnyCellEmpty(CellState[] board)
        {
            for (int i = 0; i < 9; i++) if (board[i] == CellState.Empty) return true;
            return false;
        }

        private bool IsWinnerVirtual(CellState[] board, CellState state)
        {
            for (int i = 0; i < 8; i++)
            {
                if (board[_winCombos[i, 0]] == state &&
                    board[_winCombos[i, 1]] == state &&
                    board[_winCombos[i, 2]] == state)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckWin()
        {
            var grid = _gridManager.Grid;
            for (int i = 0; i < 8; i++)
            {
                int a = _winCombos[i, 0];
                int b = _winCombos[i, 1];
                int c = _winCombos[i, 2];

                if (grid[a].CurrentState != CellState.Empty &&
                    grid[a].CurrentState == grid[b].CurrentState &&
                    grid[a].CurrentState == grid[c].CurrentState)
                {
                    if (_winLineManager != null)
                    {
                        _winLineManager.SetWinLine(_winLineTypes[i]);
                        AudioManager.Instance.PlayTicTacToeFinishGame();
                    }
                    return true;
                }
            }
            return false;
        }

        private void EndGame(Turn turn)
        {
            _isGameActive = false;
            StopAllGameCoroutines();

            if (_turnText)
                _turnText.text = (turn == Turn.Player ? "YOU WIN!" : "BOT WINS!");
            Debug.Log(turn == Turn.Player ? "Player Win" : "Bot Win");
        }
    }

    public enum Turn
    {
        Player,
        Bot
    }
}