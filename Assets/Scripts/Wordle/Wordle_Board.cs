using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace NguyenQuangMinh.Wordle
{
    public class Wordle_Board : MonoBehaviour, IGameManager
    {
        [SerializeField] private List<Wordle_Row> _rows;
        [SerializeField] private Wordle_Keyboard _keyboard;

        [Header("States")]
        [SerializeField] private State emptyState;
        [SerializeField] private State occupiedState;
        [SerializeField] private State correctState;
        [SerializeField] private State wrongSpotState;
        [SerializeField] private State incorrectState;

        [Header("Data")]
        [SerializeField] private WordDatabase wordDB;

        private List<string> solutions;
        private List<string> validWords;
        private string currentSolution;

        private int currentRow = 0;
        private int currentColumn = 0;

        private bool isInteracting = true;

        private void Start()
        {
            LoadData();
            StartNewGame();
        }

        public void LoadData()
        {
            if (solutions != null && solutions.Count > 0) return;

            if (wordDB == null)
            {
                Debug.LogError("WordDatabase is missing!");
                return;
            }
            solutions = new List<string>(wordDB.solutions);
            validWords = new List<string>(wordDB.validWords);
        }

        public void StartNewGame()
        {
            isInteracting = true;
            currentRow = 0;
            currentColumn = 0;

            foreach (var row in _rows)
            {
                foreach (var tile in row.Tiles)
                {
                    tile.ResetTile(emptyState);
                }
            }

            if (_keyboard != null) _keyboard.ResetKeyboard();

            RandomSolution();
        }

        public void Restart()
        {
            StartNewGame();
        }

        public void RandomSolution()
        {
            if (solutions == null || solutions.Count == 0) return;
            int index = Random.Range(0, solutions.Count);
            currentSolution = solutions[index].ToUpper().Trim();
            Debug.Log("Solution: " + currentSolution);
        }

        public void InputLetter(string letter)
        {
            if (!isInteracting || currentRow >= _rows.Count) return;
            if (currentColumn >= _rows[currentRow].Tiles.Count) return;

            AudioManager.Instance.PlayWordleClickSound();
            Wordle_Tile tile = _rows[currentRow].Tiles[currentColumn];
            tile.SetLetter(letter[0]);
            tile.SetState(occupiedState);

            currentColumn++;
        }

        public void DeleteLetter()
        {
            if (!isInteracting || currentRow >= _rows.Count) return;
            if (currentColumn <= 0) return;

            AudioManager.Instance.PlayWordleClickSound();
            currentColumn--;
            Wordle_Tile tile = _rows[currentRow].Tiles[currentColumn];
            tile.ResetTile(emptyState);
        }

        public void SubmitRow()
        {
            if (!isInteracting || currentRow >= _rows.Count) return;

            if (currentColumn < 5)
            {
                ShakeRow(currentRow);
                return;
            }
            string guess = GetCurrentGuess();

            if (!validWords.Contains(guess))
            {
                Debug.Log("Not in word list!");
                ShakeRow(currentRow);
                return;
            }

            AudioManager.Instance.PlayWordleSubmitSound();
            isInteracting = false;
            List<State> resultStates = EvaluateGuess(guess, currentSolution);

            Sequence submitSequence = DOTween.Sequence();

            for (int i = 0; i < 5; i++)
            {
                int index = i;
                Wordle_Tile tile = _rows[currentRow].Tiles[index];
                State targetState = resultStates[index];
                char letterChar = guess[index];

                submitSequence.Append(tile.AnimateSetState(targetState));

                submitSequence.AppendCallback(() =>
                {
                    if (targetState == incorrectState)
                    {
                        if (_keyboard != null) _keyboard.DisableKey(letterChar);
                    }
                });
            }

            submitSequence.OnComplete(() =>
            {
                if (guess == currentSolution)
                {
                    Debug.Log("YOU WIN!");
                    DOVirtual.DelayedCall(1f, () =>
                    {
                        MainGameManager.Instance.ShowWinUI(false);
                    });
                }
                else
                {
                    currentRow++;
                    currentColumn = 0;
                    if (currentRow >= _rows.Count)
                    {
                        DOVirtual.DelayedCall(1f, () =>
                        {
                            MainGameManager.Instance.ShowLoseUI();
                        });
                        Debug.Log("GAME OVER: " + currentSolution);
                    }
                    else
                    {
                        isInteracting = true;
                    }
                }
            });
        }

        private void ShakeRow(int rowIndex)
        {
            AudioManager.Instance.PlayWordleErrorSound();
            _rows[rowIndex].transform.DOShakePosition(0.5f, new Vector3(15, 0, 0), 20, 90, false, true);
        }

        List<State> EvaluateGuess(string guess, string solution)
        {
            List<State> result = new List<State>(new State[5]);
            char[] solChars = solution.ToCharArray();
            bool[] used = new bool[5];

            for (int i = 0; i < 5; i++)
            {
                if (guess[i] == solution[i])
                {
                    result[i] = correctState;
                    used[i] = true;
                    solChars[i] = '*'; 
                }
            }

            for (int i = 0; i < 5; i++)
            {
                if (result[i] != null) continue;

                bool found = false;
                for (int j = 0; j < 5; j++)
                {
                    if (!used[j] && guess[i] == solChars[j])
                    {
                        result[i] = wrongSpotState;
                        used[j] = true;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    result[i] = incorrectState;
                }
            }

            return result;
        }

        string GetCurrentGuess()
        {
            string guess = "";
            for (int i = 0; i < 5; i++)
                guess += _rows[currentRow].Tiles[i].Letter;
            return guess.ToUpper();
        }
    }
}