using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Security.Cryptography;

namespace SudokuApp.Services
{
    public static class UserManager
    {
        private static readonly string filePath = Path.Combine("Data", "users.json");
        private static List<User> users = LoadUsers();

        public static bool Login(string username, string password)
        {
            return users.Any(u => u.Username == username && u.Password == HashPassword(password));
        }

        public static bool Register(string username, string password)
        {
            if (users.Any(u => u.Username == username))
            {
                return false; // Username already taken
            }

            users.Add(new User { Username = username, Password = HashPassword(password) });
            SaveUsers();
            return true;
        }

        private static void SaveUsers()
        {
            // Ensure the directory exists before writing the file
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        private static List<User> LoadUsers()
        {
            if (!File.Exists(filePath))
                return new List<User>(); // Return empty list if file does not exist

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
}
