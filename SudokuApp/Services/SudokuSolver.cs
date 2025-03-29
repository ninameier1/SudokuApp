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

        // Main method to get a hint for the puzzle
        public static bool GetHint(SudokuPuzzle puzzle, out int hintRow, out int hintCol, out int hintValue)
        {
            hintRow = -1;
            hintCol = -1;
            hintValue = 0;

            // Solve the puzzle but stop after filling in one cell
            if (GetHintRecursive(puzzle, out hintRow, out hintCol, out hintValue))
            {
                return true;
            }
            return false;
        }

        // Recursive backtracking function to find a hint
        private static bool GetHintRecursive(SudokuPuzzle puzzle, out int hintRow, out int hintCol, out int hintValue)
        {
            // Find the first unassigned cell
            if (!FindUnassignedCell(puzzle, out hintRow, out hintCol))
            {
                hintValue = 0;
                return false; // Puzzle is fully assigned
            }

            // Try every candidate for this cell
            foreach (int num in GetCandidates(puzzle, hintRow, hintCol))
            {
                puzzle.Board[hintRow, hintCol] = num; // Tentatively assign this number
                                                      // Return the first valid number as a hint
                hintValue = num;
                return true; // We found a valid hint, return it
            }

            // If no valid number found, reset the cell and return false
            puzzle.Board[hintRow, hintCol] = 0;
            hintValue = 0;
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

    }
}
