using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SudokuApp.Models
{
    public class SaveData
    {
        public int Size { get; set; }
        public List<List<CellState>> Board { get; set; }

        // Parameterless constructor for deserialization
        public SaveData() { }

        // Constructor for creating a new SaveData object
        public SaveData(int size, CellState[,] board)
        {
            Size = size;
            Board = new List<List<CellState>>();

            // Convert the 2D array into a List<List<CellState>>
            for (int i = 0; i < board.GetLength(0); i++)
            {
                List<CellState> row = new List<CellState>();
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    row.Add(board[i, j]);
                }
                Board.Add(row);
            }
        }
    }
}



