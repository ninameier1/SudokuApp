using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace SudokuApp.Services
{
    public static class UserManager
    {
        private static List<User> users = new List<User>
        {
            new User { Username = "admin", Password = "admin" } // Temporary user
        };

        public static bool Login(string username, string password)
        {
            return users.Any(u => u.Username == username && u.Password == password);
        }

        public static bool Register(string username, string password)
        {
            // Check if the username already exists
            if (users.Any(u => u.Username == username))
            {
                return false; // Username already taken
            }

            users.Add(new User { Username = username, Password = password });
            return true; // User registered successfully
        }
    }
}

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
}

