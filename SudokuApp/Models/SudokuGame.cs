//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SudokuApp.Models
//{
//    public class SudokuGame

//    {

//            public int ID { get; set; }
//            public string PlayerName { get; set; }
//            public string GridState { get; set; }
//            public int TimeElapsed { get; set; }

//    }


//}

namespace SudokuApp.Models
{
    public class SudokuGame
    {
        public int ID { get; set; }
        public string PlayerName { get; set; } = string.Empty; // Initialiseer met default waarde
        public string GridState { get; set; } = string.Empty;  // Initialiseer met default waarde
        public int TimeElapsed { get; set; }
    }
}