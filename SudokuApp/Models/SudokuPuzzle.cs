using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuApp.Models
{
    public class SudokuPuzzle
    {
        public int[,] Board { get; set; }
        public int Size { get; private set; }

        public SudokuPuzzle(int size)
        {
            Size = size;
            Board = new int[size, size];
        }

        public SudokuPuzzle Clone()
        {
            SudokuPuzzle clone = new SudokuPuzzle(Size);
            System.Array.Copy(Board, clone.Board, Board.Length);
            return clone;
        }

    }
}
