using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace NguyenQuangMinh.Battleship
{
    public enum GameState_Battleship { Setup, PlayerTurn, EnemyTurn, GameOver }

    public class GameManagerBattleship : Singleton<GameManagerBattleship>
    {
        [Header("UI Setting")]
        [SerializeField] private TextMeshProUGUI turnText;
        [SerializeField] private TextMeshProUGUI playerScoreText;
        [SerializeField] private TextMeshProUGUI enemyScoreText;

        [Header("Board Setting")]
        [SerializeField] private Board playerBoard;
        [SerializeField] private Board enemyBoard;

        // Trạng thái game
        private GameState_Battleship currentState;
        private int playerScore = 0;
        private int enemyScore = 0;
        private int totalShipParts = 0;

        // Lưu các ô đã bắn
        private List<Vector2Int> playerShots = new List<Vector2Int>();
        private List<Vector2Int> enemyShots = new List<Vector2Int>();

        void Start()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            totalShipParts = CalculateTotalShipParts(enemyBoard);
            StartPlayerTurn();
        }

        private int CalculateTotalShipParts(Board board)
        {
            int total = 0;
            foreach (var ship in board.ShipList)
            {
                total += ship.Length;
            }
            return total;
        }

        public void ProcessPlayerAttack(Vector2Int coord)
        {
            if (currentState != GameState_Battleship.PlayerTurn) return;
            if (playerShots.Contains(coord)) return;

            playerShots.Add(coord);
            bool isHit = enemyBoard.CheckHit(coord);

            if (isHit)
            {
                playerScore++;
                UpdateUI();

                if (playerScore >= totalShipParts)
                {
                    EndGame(true);
                    return;
                }
            }
            else
            {
                StartEnemyTurn();
            }
        }

        private void StartPlayerTurn()
        {
            currentState = GameState_Battleship.PlayerTurn;
            UpdateUI();
        }

        private void StartEnemyTurn()
        {
            currentState = GameState_Battleship.EnemyTurn;
            UpdateUI();
            StartCoroutine(EnemyTurnRoutine());
        }

        private IEnumerator EnemyTurnRoutine()
        {
            yield return new WaitForSeconds(1f);

            Vector2Int target = GetAITarget();
            enemyShots.Add(target);
            bool isHit = playerBoard.CheckHit(target);

            if (isHit)
            {
                enemyScore++;
                UpdateUI();

                if (enemyScore >= totalShipParts)
                {
                    EndGame(false);
                    yield break;
                }

                // Nếu trúng, AI được bắn tiếp
                StartCoroutine(EnemyTurnRoutine());
            }
            else
            {
                // Nếu trượt, đến lượt người chơi
                StartPlayerTurn();
            }
        }

        private Vector2Int GetAITarget()
        {
            // Ưu tiên bắn quanh các ô đã trúng
            foreach (var hitCoord in enemyShots)
            {
                var cell = playerBoard.Grid.GetCell(hitCoord);
                if (cell != null && cell.HasShipPart)
                {
                    var neighbors = new List<Vector2Int>
                {
                    hitCoord + Vector2Int.up,
                    hitCoord + Vector2Int.down,
                    hitCoord + Vector2Int.left,
                    hitCoord + Vector2Int.right
                };

                    foreach (var coord in neighbors)
                    {
                        if (playerBoard.Grid.IsValidCoord(coord) &&
                            !enemyShots.Contains(coord))
                        {
                            return coord;
                        }
                    }
                }
            }

            return GetRandomValidCoord();
        }

        private Vector2Int GetRandomValidCoord()
        {
            List<Vector2Int> validCoords = new List<Vector2Int>();

            for (int x = 0; x < playerBoard.Grid.GetWidth(); x++)
            {
                for (int y = 0; y < playerBoard.Grid.GetHeight(); y++)
                {
                    var coord = new Vector2Int(x, y);
                    if (!enemyShots.Contains(coord))
                    {
                        validCoords.Add(coord);
                    }
                }
            }

            return validCoords[Random.Range(0, validCoords.Count)];
        }

        private void EndGame(bool playerWon)
        {
            currentState = GameState_Battleship.GameOver;
            turnText.text = playerWon ? "You Win!" : "You Lose!";
        }

        private void UpdateUI()
        {
            playerScoreText.text = $"Player: {playerScore}";
            enemyScoreText.text = $"Enemy: {enemyScore}";
            turnText.text = currentState == GameState_Battleship.PlayerTurn ? "Your Turn" : "Enemy's Turn";
        }

        public bool IsPlayerTurn() => currentState == GameState_Battleship.PlayerTurn;
    }
}