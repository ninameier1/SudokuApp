using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuApp.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public UserSettings Settings { get; set; } = new UserSettings();
    }

    public class UserSettings
    {
        public string Difficulty { get; set; } = "4x4";
    }

}
