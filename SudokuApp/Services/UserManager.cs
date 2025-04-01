using System;
using System.IO;
using System.Text.Json;
using SudokuApp.Models;
using System.Security.Cryptography;

namespace SudokuApp.Services
{
    public static class UserManager
    {
        private static readonly string usersDirectory = Path.Combine("Data", "Users");

        public static bool Login(string username, string password)
        {
            User? user = LoadUser(username);
            return user != null && user.Password == HashPassword(password);
        }

        public static bool Register(string username, string password)
        {
            if (LoadUser(username) != null)
            {
                return false; // Username already taken
            }

            User newUser = new User { Username = username, Password = HashPassword(password) };
            SaveUser(newUser);
            return true;
        }

        // New: Update user credentials (username and/or password)
        public static bool UpdateUser(string oldUsername, string newUsername, string newPassword)
        {
            // Check if new username is already taken (if changed)
            if (!oldUsername.Equals(newUsername, StringComparison.OrdinalIgnoreCase) && LoadUser(newUsername) != null)
                return false;

            User? user = LoadUser(oldUsername);
            if (user == null)
                return false;

            // If username changes, delete the old file and update the username property.
            if (!oldUsername.Equals(newUsername, StringComparison.OrdinalIgnoreCase))
            {
                DeleteUser(oldUsername);
                user.Username = newUsername;
            }
            // Update password with new hash
            user.Password = HashPassword(newPassword);

            SaveUser(user);
            return true;
        }

        // New: Delete a user's account
        public static bool DeleteUser(string username)
        {
            string userFilePath = Path.Combine(usersDirectory, $"{username}.json");
            if (File.Exists(userFilePath))
            {
                File.Delete(userFilePath);
                return true;
            }
            return false;
        }

        private static void SaveUser(User user)
        {
            Directory.CreateDirectory(usersDirectory);
            string userFilePath = Path.Combine(usersDirectory, $"{user.Username}.json");
            string json = JsonSerializer.Serialize(user, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(userFilePath, json);
        }

        private static User? LoadUser(string username)
        {
            string userFilePath = Path.Combine(usersDirectory, $"{username}.json");
            if (!File.Exists(userFilePath))
                return null;

            string json = File.ReadAllText(userFilePath);
            return JsonSerializer.Deserialize<User>(json);
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
