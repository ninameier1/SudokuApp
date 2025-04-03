using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace SudokuApp.Models
{
    public class CellState
    {
        public int Value { get; set; }
        public bool IsEditable { get; set; }

        // Parameterless constructor for deserialization
        public CellState() { }

        // Constructor for creating new instances
        public CellState(int value, bool isEditable)
        {
            Value = value;
            IsEditable = isEditable;
        }
    }

}

