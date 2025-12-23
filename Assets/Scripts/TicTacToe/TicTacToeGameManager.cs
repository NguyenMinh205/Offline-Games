using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

namespace NguyenQuangMinh.TicTacToe
{
    public class TicTacToeGameManager : Singleton<TicTacToeGameManager>, IGameManager
    {
        [Header("References")]
        [SerializeField] private TicTacToeGridManager _gridManager;
        [SerializeField] private WinLineManager _winLineManager; // Đã có sẵn reference này từ code bạn gửi
        // Đã xóa LineRenderer

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
        private Turn _currentTurn;
        private bool _isGameActive = false;

        private void Start()
        {
            StartNewGame();
        }

        public void StartNewGame()
        {
            _gridManager.SetupGrid();

            if (_winLineManager != null)
                _winLineManager.SetWinLine(WinLine.None);

            int firstTurn = Random.Range(0, 2);
            _currentTurn = (firstTurn == 0) ? Turn.Player : Turn.Bot;

            if (_currentTurn == Turn.Player)
            {
                _playerState = CellState.X;
                _botState = CellState.O;
            }
            else
            {
                _playerState = CellState.O;
                _botState = CellState.X;
            }

            _isGameActive = true;

            if (_currentTurn == Turn.Bot)
            {
                StartCoroutine(BotTurnRoutine());
            }
        }

        public void CellClicked(int cellID)
        {
            if (!_isGameActive || _currentTurn != Turn.Player) return;
            if (_gridManager.GetCellControllerAt(cellID).CurrentState != CellState.Empty) return;

            ProcessTurn(Turn.Player, cellID);
        }

        private void ProcessTurn(Turn turn, int selectedCell)
        {
            CellState state = (turn == Turn.Player) ? _playerState : _botState;
            _gridManager.SetSpecificCell(state, selectedCell);

            if (CheckWin())
            {
                EndGame(turn);
                return;
            }

            if (_gridManager.CheckFullGrid())
            {
                StartCoroutine(HandleDrawRoutine());
                return;
            }

            ChangeTurn();
        }

        private IEnumerator HandleDrawRoutine()
        {
            _isGameActive = false;
            yield return new WaitForSeconds(1f);

            _gridManager.SetupGrid();

            if (_winLineManager != null)
                _winLineManager.SetWinLine(WinLine.None);

            _isGameActive = true;
            ChangeTurn();
        }

        private void ChangeTurn()
        {
            _currentTurn = (_currentTurn == Turn.Player) ? Turn.Bot : Turn.Player;

            if (_currentTurn == Turn.Bot)
            {
                StartCoroutine(BotTurnRoutine());
            }
        }

        private IEnumerator BotTurnRoutine()
        {
            yield return new WaitForSeconds(_botDelay);
            int move = GetBotMove();
            ProcessTurn(Turn.Bot, move);
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
                int randomIndex = Random.Range(0, availableMoves.Count);
                return availableMoves[randomIndex];
            }
        }

        private int Minimax(CellState[] board, int depth, bool isMaximizing)
        {
            if (IsWinnerVirtual(board, _botState)) return 10 - depth;
            if (IsWinnerVirtual(board, _playerState)) return depth - 10;
            if (_gridManager.CheckFullGrid()) return 0;

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
                    }
                    return true;
                }
            }
            return false;
        }

        private void EndGame(Turn turn)
        {
            _isGameActive = false;
            Debug.Log(turn == Turn.Player ? "Player Win" : "Bot Win");
        }

        public void Restart()
        {
            StartNewGame();
        }
    }

    public enum Turn
    {
        Player,
        Bot
    }
}