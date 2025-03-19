using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuApp.Models;

namespace SudokuApp.Services
{
    public static class SudokuSolver
    {
        public static bool Solve(SudokuPuzzle puzzle)
        {
            return SolveRecursive(puzzle, 0, 0);
        }

        private static bool SolveRecursive(SudokuPuzzle puzzle, int row, int col)
        {
            int size = puzzle.Size;
            if (row == size)
                return true;
            int nextRow = col == size - 1 ? row + 1 : row;
            int nextCol = col == size - 1 ? 0 : col + 1;
            if (puzzle.Board[row, col] != 0)
                return SolveRecursive(puzzle, nextRow, nextCol);

            for (int num = 1; num <= size; num++)
            {
                if (IsValid(puzzle, row, col, num))
                {
                    puzzle.Board[row, col] = num;
                    if (SolveRecursive(puzzle, nextRow, nextCol))
                        return true;
                    puzzle.Board[row, col] = 0;
                }
            }
            return false;
        }

        private static bool IsValid(SudokuPuzzle puzzle, int row, int col, int num)
        {
            int size = puzzle.Size;
            int sqrt = (int)System.Math.Sqrt(size);

            for (int j = 0; j < size; j++)
                if (puzzle.Board[row, j] == num)
                    return false;

            for (int i = 0; i < size; i++)
                if (puzzle.Board[i, col] == num)
                    return false;

            int startRow = row - row % sqrt;
            int startCol = col - col % sqrt;
            for (int i = 0; i < sqrt; i++)
                for (int j = 0; j < sqrt; j++)
                    if (puzzle.Board[startRow + i, startCol + j] == num)
                        return false;
            return true;
        }
    }
}
