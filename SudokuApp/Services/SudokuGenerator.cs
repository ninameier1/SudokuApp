﻿using System;
using System.Threading;
using SudokuApp.Models;

namespace SudokuApp.Services
{
    // Static class to handle Sudoku puzzle generation logic
    public static class SudokuGenerator
    {
        // Shared Random instance for better randomness and efficiency
        private static Random rand = new Random();

        // CancellationTokenSource to handle task cancellation
        private static CancellationTokenSource _cancellationTokenSource = null;

        // Main method to generate a new Sudoku puzzle
        public static SudokuPuzzle Generate(int size, CancellationToken cancellationToken)
        {
            // Create a new Sudoku puzzle using the model's constructor (size defines the puzzle's dimensions)
            SudokuPuzzle puzzle = new SudokuPuzzle(size);

            // Try to fill the board using a backtracking approach. If it fails, throw an exception
            if (!FillBoard(puzzle, cancellationToken))
            {
                throw new Exception("Unable to generate board.");
            }

            // Generate a random number to vary the difficulty of the puzzle
            int variant = rand.Next(12);

            // Determine total cells on the board
            int totalCells = size * size;

            // For a more controlled removal, define a minimum number of clues to leave.
            int minClues;
            if (size == 4)
            {
                // For a 4x4, leave at least 8 clues (adjust as needed)
                minClues = 8;
            }
            else if (size == 9)
            {
                // For a 9x9, you might want at least 17 clues (minimum known for a unique sudoku)
                minClues = 17;
            }
            else if (size == 16)
            {
                // For a 16x16, choose a value that makes sense for your puzzle design
                minClues = 40; // for example
            }
            else
            {
                // Default fallback
                minClues = totalCells / 2;
            }

            // Calculate removal count with a variant and cap it so we leave at least minClues on the board.
            int removeCount = Math.Min((totalCells / 2) + variant, totalCells - minClues);

            // Remove a random set of cells from the board to create the puzzle
            RemoveCells(puzzle, removeCount, cancellationToken);


            // Return the generated Sudoku puzzle
            return puzzle;
        }

        private static bool FillBoard(SudokuPuzzle puzzle, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested(); //  Check cancellation before each step

            var bestCell = FindBestCell(puzzle);
            if (bestCell == null)
                return true; // Board is filled

            int row = bestCell.Value.row;
            int col = bestCell.Value.col;

            int[] numbers = GetShuffledNumbers(puzzle.Size);

            foreach (int num in numbers)
            {
                if (IsValid(puzzle, row, col, num))
                {
                    puzzle.Board[row, col] = num;

                    if (FillBoard(puzzle, cancellationToken))
                        return true;
                }
            }

            puzzle.Board[row, col] = 0;
            return false;
        }

        private static (int row, int col)? FindBestCell(SudokuPuzzle puzzle)
        {
            int size = puzzle.Size;
            int minOptions = size + 1;
            (int row, int col)? bestCell = null;

            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    if (puzzle.Board[row, col] == 0) // Find an empty cell
                    {
                        int options = 0;
                        for (int num = 1; num <= size; num++)
                        {
                            if (IsValid(puzzle, row, col, num))
                                options++;
                        }

                        if (options < minOptions)
                        {
                            minOptions = options;
                            bestCell = (row, col);
                        }
                    }
                }
            }

            return bestCell;
        }



        // Helper method to generate a shuffled list of numbers from 1 to 'size'
        private static int[] GetShuffledNumbers(int size)
        {
            int[] numbers = new int[size];

            // Populate the array with numbers from 1 to 'size'
            for (int i = 0; i < size; i++)
                numbers[i] = i + 1;

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
        private static void RemoveCells(SudokuPuzzle puzzle, int count, CancellationToken cancellationToken)
        {
            int size = puzzle.Size;

            // While there are still cells to remove
            while (count > 0)
            {
                // Check for cancellation
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

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

        // This method will cancel any ongoing task before starting a new one
        public static void CancelPreviousTask()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public static async Task<SudokuPuzzle> GeneratePuzzleAsync(int gridSize, CancellationToken cancellationToken)
        {
            // Wrap puzzle generation in an asynchronous call
            return await Task.Run(() => SudokuGenerator.Generate(gridSize, cancellationToken), cancellationToken);
        }
    }
}
