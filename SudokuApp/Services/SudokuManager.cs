using System;
using System.IO;
using System.Text.Json;

namespace SudokuApp.Services
{
    public static class SudokuManager
    {
        public static void SaveGame(string username, int[,] board)
        {
            string filePath = $"saves/{username}_sudoku.json"; // Saves in a 'saves' folder
            Directory.CreateDirectory("saves"); // Ensure the folder exists

            string json = JsonSerializer.Serialize(board);
            File.WriteAllText(filePath, json);
        }

        public static int[,] LoadGame(string username)
        {
            string filePath = $"saves/{username}_sudoku.json";
            if (!File.Exists(filePath)) return new int[9, 9]; // Return empty board if no saved game

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<int[,]>(json);
        }
    }
}
