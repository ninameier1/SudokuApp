using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuApp.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty; // Initialiseer met default waarde

        [Required]
        public string Password { get; set; } = string.Empty; // Initialiseer met default waarde

        public int TotalMinutesPlayed { get; set; }
        public int PuzzlesSolved { get; set; }
        public int LeaderboardRank { get; set; }
        public float SuccessRate { get; set; }
    }
}


