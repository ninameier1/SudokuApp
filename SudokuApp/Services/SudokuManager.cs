using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SudokuApp.Models;

namespace SudokuApp.Services
{
    public static class SudokuManager
    {
        public static void SaveGame(string username, CellState[,] board, int gridSize)
        {
            string filePath = $"saves/{username}_sudoku.json";
            Directory.CreateDirectory("saves");

            // Create the SaveData object
            var saveData = new SaveData(gridSize, board);

            // Serialize the SaveData object to JSON
            string json = JsonSerializer.Serialize(saveData);

            // Write the JSON to a file
            File.WriteAllText(filePath, json);
        }


        public static (CellState[,] board, int gridSize) LoadGame(string username)
        {
            string filePath = $"saves/{username}_sudoku.json";

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Game save file not found.");
            }

            string json = File.ReadAllText(filePath);

            // Deserialize the JSON into the SaveData object
            var saveData = JsonSerializer.Deserialize<SaveData>(json);

            // Extract the grid size and board
            int gridSize = saveData.Size;
            var boardList = saveData.Board;

            // Convert the List<List<CellState>> back to a 2D array of CellState
            int rows = boardList.Count;
            int cols = boardList[0].Count;
            CellState[,] board = new CellState[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    board[i, j] = boardList[i][j];
                }
            }

            return (board, gridSize);  // Return both board and gridSize
        }


    }
}
