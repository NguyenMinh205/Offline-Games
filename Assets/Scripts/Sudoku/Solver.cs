using System;
using System.Collections;
using System.Collections.Generic;

namespace NguyenQuangMinh.Sudoku
{
    public class Solver
    {
        private const int BOARD_SIZE = 9;
        private const int SUBGRID_SIZE = 3;
        private const int EMPTY_CELL = 0;

        public static int[,] SolveSudoku(int[,] unsolvedPuzzle)
        {
            int[,] solvedPuzzle = new int[BOARD_SIZE, BOARD_SIZE];
            Array.Copy(unsolvedPuzzle, 0, solvedPuzzle, 0, unsolvedPuzzle.Length);
            BackTrack(solvedPuzzle, 0, 0);
            return solvedPuzzle;
        }

        private static bool BackTrack(int[,] puzzle, int row, int col)
        {
            if (row == BOARD_SIZE)
            {
                return true;
            }

            if (col == BOARD_SIZE)
            {
                return BackTrack(puzzle, row + 1, 0);
            }

            if (puzzle[row, col] != 0)
            {
                return BackTrack(puzzle, row, col + 1);
            }

            for (int i = 1; i <= BOARD_SIZE; i++)
            {
                if (IsValid(puzzle, row, col, i))
                {
                    puzzle[row, col] = i;
                    if (BackTrack(puzzle, row, col + 1))
                    {
                        return true;
                    }
                    puzzle[row, col] = 0;
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

        public static bool HasUniqueSolution(int[,] board)
        {
            int[,] copy = new int[BOARD_SIZE, BOARD_SIZE];
            Array.Copy(board, 0, copy, 0, board.Length);
            int solutionCount = 0;
            CountSolutions(copy, 0, 0, ref solutionCount);
            return solutionCount == 1;
        }

        private static bool CountSolutions(int[,] puzzle, int row, int col, ref int solutionCount)
        {
            if (solutionCount > 1) return false;

            if (row == BOARD_SIZE)
            {
                solutionCount++;
                return solutionCount <= 1;
            }

            if (col == BOARD_SIZE)
            {
                return CountSolutions(puzzle, row + 1, 0, ref solutionCount);
            }

            if (puzzle[row, col] != EMPTY_CELL)
            {
                return CountSolutions(puzzle, row, col + 1, ref solutionCount);
            }

            for (int val = 1; val <= BOARD_SIZE; val++)
            {
                if (IsValid(puzzle, row, col, val))
                {
                    puzzle[row, col] = val;
                    if (!CountSolutions(puzzle, row, col + 1, ref solutionCount))
                    {
                        puzzle[row, col] = EMPTY_CELL;
                        return false;
                    }
                    puzzle[row, col] = EMPTY_CELL;
                }
            }

            return true;
        }
    }
}