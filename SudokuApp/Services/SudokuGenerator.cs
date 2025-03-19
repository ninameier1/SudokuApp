using System;
using SudokuApp.Models;

namespace SudokuApp.Services
{
    // Static class to handle Sudoku puzzle generation logic
    public static class SudokuGenerator
    {
        // Main method to generate a new Sudoku puzzle
        public static SudokuPuzzle Generate(int size)
        {
            // Create a new Sudoku puzzle using the model's constructor (size defines the puzzle's dimensions)
            SudokuPuzzle puzzle = new SudokuPuzzle(size);

            // Try to fill the board using a backtracking approach. If it fails, throw an exception
            if (!FillBoard(puzzle, 0, 0))
            {
                throw new Exception("Unable to generate board.");
            }

            // Create a random number generator
            Random rand = new Random();

            // Generate a random number to vary the difficulty of the puzzle
            int variant = rand.Next(12);

            // Calculate how many cells to remove, making the puzzle more challenging
            int removeCount = (size * size) / 2 + variant;

            // Remove a random set of cells from the board to create the puzzle
            RemoveCells(puzzle, removeCount);

            // Return the generated Sudoku puzzle
            return puzzle;
        }

        // Recursive method to fill the Sudoku board using backtracking
        private static bool FillBoard(SudokuPuzzle puzzle, int row, int col)
        {
            int size = puzzle.Size;

            // If we reached the last row, the board is successfully filled
            if (row == size)
                return true;

            // Calculate the next row and column to move to
            int nextRow = col == size - 1 ? row + 1 : row;
            int nextCol = col == size - 1 ? 0 : col + 1;

            // Get a shuffled array of possible numbers for the current cell
            int[] numbers = GetShuffledNumbers(size);

            // Try placing each number in the current cell
            foreach (int num in numbers)
            {
                // If the number is valid in this position, set it
                if (IsValid(puzzle, row, col, num))
                {
                    puzzle.Board[row, col] = num;

                    // Recursively try to fill the next cell
                    if (FillBoard(puzzle, nextRow, nextCol))
                        return true;
                }
            }

            // If no valid number can be placed, backtrack and reset the current cell
            puzzle.Board[row, col] = 0;
            return false;
        }

        // Helper method to generate a shuffled list of numbers from 1 to 'size'
        private static int[] GetShuffledNumbers(int size)
        {
            int[] numbers = new int[size];

            // Populate the array with numbers from 1 to 'size'
            for (int i = 0; i < size; i++)
                numbers[i] = i + 1;

            // Create a random number generator
            Random rand = new Random();

            // Shuffle the numbers using the Fisher-Yates shuffle algorithm
            for (int i = 0; i < size; i++)
            {
                int j = rand.Next(size);
                int temp = numbers[i];
                numbers[i] = numbers[j];
                numbers[j] = temp;
            }

            return numbers;
        }

        // Method to check if a number can be placed at a given position in the Sudoku grid
        private static bool IsValid(SudokuPuzzle puzzle, int row, int col, int num)
        {
            int size = puzzle.Size;
            int sqrt = (int)Math.Sqrt(size); // Calculate the size of the subgrid (block)

            // Check if the number already exists in the current row
            for (int j = 0; j < size; j++)
                if (puzzle.Board[row, j] == num)
                    return false;

            // Check if the number already exists in the current column
            for (int i = 0; i < size; i++)
                if (puzzle.Board[i, col] == num)
                    return false;

            // Check if the number already exists in the current subgrid/block
            int startRow = row - row % sqrt;
            int startCol = col - col % sqrt;
            for (int i = 0; i < sqrt; i++)
                for (int j = 0; j < sqrt; j++)
                    if (puzzle.Board[startRow + i, startCol + j] == num)
                        return false;

            // If the number is not found in the row, column, or block, it's valid
            return true;
        }

        // Method to remove a specific number of cells from the board to create a puzzle
        private static void RemoveCells(SudokuPuzzle puzzle, int count)
        {
            // Create a random number generator
            Random rand = new Random();
            int size = puzzle.Size;

            // While there are still cells to remove
            while (count > 0)
            {
                // Pick a random cell position
                int row = rand.Next(size);
                int col = rand.Next(size);

                // If the cell isn't already empty, remove the number by setting it to 0
                if (puzzle.Board[row, col] != 0)
                {
                    puzzle.Board[row, col] = 0;
                    count--; // Decrement the number of remaining cells to remove
                }
            }
        }
    }
}
