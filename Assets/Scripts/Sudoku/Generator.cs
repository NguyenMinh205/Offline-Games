using System;
using System.Collections;
using System.Collections.Generic;

namespace NguyenQuangMinh.Sudoku
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    public class Generator
    {
        private const int BOARD_SIZE = 9;
        private const int SUBGRID_SIZE = 3;
        private const int MIN_CELL_EMPTY = 15;
        private const int MAX_CELL_EMPTY = 45;

        private static readonly Random rnd = new Random();

        public static int[,] GeneratePuzzle(Difficulty difficulty)
        {
            int[,] board = new int[BOARD_SIZE, BOARD_SIZE];
            int cellsToRemove = 0;
            switch (difficulty)
            {
                case Difficulty.Easy:
                    cellsToRemove = rnd.Next(MIN_CELL_EMPTY, MIN_CELL_EMPTY + 10);
                    break;
                case Difficulty.Medium:
                    cellsToRemove = rnd.Next(MIN_CELL_EMPTY + 10, MIN_CELL_EMPTY + 20);
                    break;
                case Difficulty.Hard:
                    cellsToRemove = rnd.Next(MIN_CELL_EMPTY + 20, MAX_CELL_EMPTY);
                    break;
                default:
                    break;
            }

            InitializeBoard(board);

            while (cellsToRemove > 0)
            {
                int row = rnd.Next(0, BOARD_SIZE);
                int col = rnd.Next(0, BOARD_SIZE);
                if (board[row, col] != 0)
                {
                    int temp = board[row, col];
                    board[row, col] = 0;
                    if (Solver.HasUniqueSolution(board))
                    {
                        cellsToRemove--;
                    }
                    else
                    {
                        board[row, col] = temp;
                    }
                }
            }
            return board;
        }

        private static void InitializeBoard(int[,] board)
        {
            List<int> numbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Shuffle(numbers);
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                board[0, i] = numbers[i];
            }
            FillGrid(board, 1, 0);
        }

        private static bool FillGrid(int[,] board, int row, int col)
        {
            if (row == BOARD_SIZE)
            {
                return true;
            }
            if (col == BOARD_SIZE)
            {
                return FillGrid(board, row + 1, 0);
            }

            List<int> numbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Shuffle(numbers);
            foreach (int num in numbers)
            {
                if (IsValid(board, row, col, num))
                {
                    board[row, col] = num;
                    if (FillGrid(board, row, col + 1))
                    {
                        return true;
                    }
                    board[row, col] = 0;
                }
            }
            return false;
        }

        private static bool IsValid(int[,] board, int row, int col, int val)
        {

            for (int i = 0; i < BOARD_SIZE; i++)
            {
                if (board[row, i] == val)
                {
                    return false;
                }
            }

            for (int i = 0; i < BOARD_SIZE; i++)
            {
                if (board[i, col] == val)
                {
                    return false;
                }
            }

            int subGridRow = row / SUBGRID_SIZE * SUBGRID_SIZE;
            int subGridCol = col / SUBGRID_SIZE * SUBGRID_SIZE;
            for (int r = subGridRow; r < subGridRow + SUBGRID_SIZE; r++)
            {
                for (int c = subGridCol; c < subGridCol + SUBGRID_SIZE; c++)
                {
                    if (board[r, c] == val)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static void Shuffle(List<int> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                int k = rnd.Next(n--);
                int temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }
        }
    }

}