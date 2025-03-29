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
            return SolveRecursive(puzzle);
        }

        //private static bool SolveRecursive(SudokuPuzzle puzzle, int row, int col)
        //{
        //    int size = puzzle.Size;
        //    if (row == size)
        //        return true;
        //    int nextRow = col == size - 1 ? row + 1 : row;
        //    int nextCol = col == size - 1 ? 0 : col + 1;
        //    if (puzzle.Board[row, col] != 0)
        //        return SolveRecursive(puzzle, nextRow, nextCol);

        //    for (int num = 1; num <= size; num++)
        //    {
        //        if (IsValid(puzzle, row, col, num))
        //        {
        //            puzzle.Board[row, col] = num;
        //            if (SolveRecursive(puzzle, nextRow, nextCol))
        //                return true;
        //            puzzle.Board[row, col] = 0;
        //        }
        //    }
        //    return false;
        //}


        // Example: Find next cell with minimum candidates
        private static bool SolveRecursive(SudokuPuzzle puzzle)
        {
            // Find the unassigned cell with the fewest candidates
            if (!FindUnassignedCell(puzzle, out int row, out int col))
                return true; // puzzle solved

            foreach (int num in GetCandidates(puzzle, row, col))
            {
                puzzle.Board[row, col] = num;
                if (SolveRecursive(puzzle))
                    return true;
                puzzle.Board[row, col] = 0;
            }
            return false;
        }

        // Helper methods to find unassigned cell and get its candidate values:
        private static bool FindUnassignedCell(SudokuPuzzle puzzle, out int minRow, out int minCol)
        {
            int size = puzzle.Size;
            int minCount = int.MaxValue;
            minRow = -1; minCol = -1;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (puzzle.Board[i, j] == 0)
                    {
                        int count = GetCandidates(puzzle, i, j).Count();
                        if (count < minCount)
                        {
                            minCount = count;
                            minRow = i;
                            minCol = j;
                            // Early exit if there is a cell with only one candidate
                            if (minCount == 1) return true;
                        }
                    }
                }
            }
            return minRow != -1;
        }

        private static IEnumerable<int> GetCandidates(SudokuPuzzle puzzle, int row, int col)
        {
            int size = puzzle.Size;
            var candidates = new bool[size + 1];
            for (int k = 1; k <= size; k++) candidates[k] = true;

            // Eliminate candidates based on row and column
            for (int i = 0; i < size; i++)
            {
                if (puzzle.Board[row, i] != 0)
                    candidates[puzzle.Board[row, i]] = false;
                if (puzzle.Board[i, col] != 0)
                    candidates[puzzle.Board[i, col]] = false;
            }

            // Eliminate candidates based on subgrid
            int sqrt = (int)Math.Sqrt(size);
            int startRow = row - row % sqrt;
            int startCol = col - col % sqrt;
            for (int i = 0; i < sqrt; i++)
            {
                for (int j = 0; j < sqrt; j++)
                {
                    if (puzzle.Board[startRow + i, startCol + j] != 0)
                        candidates[puzzle.Board[startRow + i, startCol + j]] = false;
                }
            }

            // Return the numbers still available
            for (int num = 1; num <= size; num++)
            {
                if (candidates[num])
                    yield return num;
            }
        }

        //private static bool IsValid(SudokuPuzzle puzzle, int row, int col, int num)
        //{
        //    int size = puzzle.Size;
        //    int sqrt = (int)System.Math.Sqrt(size);

        //    for (int j = 0; j < size; j++)
        //        if (puzzle.Board[row, j] == num)
        //            return false;

        //    for (int i = 0; i < size; i++)
        //        if (puzzle.Board[i, col] == num)
        //            return false;

        //    int startRow = row - row % sqrt;
        //    int startCol = col - col % sqrt;
        //    for (int i = 0; i < sqrt; i++)
        //        for (int j = 0; j < sqrt; j++)
        //            if (puzzle.Board[startRow + i, startCol + j] == num)
        //                return false;
        //    return true;
        //}
    }
    }
