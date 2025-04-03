using System;
using System.IO;
using System.Text.Json;
using SudokuApp.Models;
using System.Security.Cryptography;
using SudokuApp.Utils;

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

        //// Update user credentials (username and/or password)
        //public static bool UpdateUser(string oldUsername, string newUsername, string newPassword)
        //{
        //    // Check if new username is already taken (if changed)
        //    if (!oldUsername.Equals(newUsername, StringComparison.OrdinalIgnoreCase) && LoadUser(newUsername) != null)
        //        return false;

        //    User? user = LoadUser(oldUsername);
        //    if (user == null)
        //        return false;

        //    // If username changes, delete the old file and update the username property.
        //    if (!oldUsername.Equals(newUsername, StringComparison.OrdinalIgnoreCase))
        //    {
        //        DeleteUser(oldUsername);
        //        user.Username = newUsername;
        //    }
        //    // Update password with new hash
        //    user.Password = HashPassword(newPassword);

        //    SaveUser(user);
        //    return true;
        //}

        //public static bool UpdateUser(string oldUsername, string newUsername, string newPassword)
        //{
        //    // Check if new username is already taken (if changed)
        //    if (!oldUsername.Equals(newUsername, StringComparison.OrdinalIgnoreCase) && LoadUser(newUsername) != null)
        //        return false;

        //    User? user = LoadUser(oldUsername);
        //    if (user == null)
        //        return false;

        //    // If username changes, delete the old file and update the username property.
        //    if (!oldUsername.Equals(newUsername, StringComparison.OrdinalIgnoreCase))
        //    {
        //        // Ensure the old file is deleted before updating
        //        bool deleteSuccess = DeleteUser(oldUsername);
        //        if (!deleteSuccess)
        //        {
        //            return false;  // If deleting the old user file failed, return false
        //        }

        //        user.Username = newUsername;
        //    }

        //    // Update password with new hash
        //    user.Password = HashPassword(newPassword);

        //    // Save the updated user data
        //    SaveUser(user);

        //    return true;
        //}

        //public static bool UpdateUser(string oldUsername, string newUsername, string newPassword)
        //{
        //    // Check if new username is already taken (if changed)
        //    if (!oldUsername.Equals(newUsername, StringComparison.OrdinalIgnoreCase) && LoadUser(newUsername) != null)
        //        return false;

        //    User? user = LoadUser(oldUsername);
        //    if (user == null)
        //        return false;

        //    // If username changes, delete the old file and update the username property.
        //    if (!oldUsername.Equals(newUsername, StringComparison.OrdinalIgnoreCase))
        //    {
        //        // Ensure the old file is deleted before updating
        //        bool deleteSuccess = DeleteUser(oldUsername);
        //        if (!deleteSuccess)
        //        {
        //            return false;  // If deleting the old user file failed, return false
        //        }

        //        // Update the user's username
        //        user.Username = newUsername;
        //    }

        //    // Update password with new hash
        //    user.Password = HashPassword(newPassword);

        //    // Save the updated user data
        //    SaveUser(user);

        //    // Now update the _currentUser in memory (the logged-in user)
        //    if (_currentUser != null)
        //    {
        //        _currentUser.Username = newUsername;  // Update the logged-in user with the new username
        //    }

        //    return true;
        //}

        //public static bool UpdateUser(User currentUser, string newUsername, string newPassword)
        //{
        //    // Check if new username is already taken (if changed)
        //    if (!currentUser.Username.Equals(newUsername, StringComparison.OrdinalIgnoreCase) && LoadUser(newUsername) != null)
        //        return false;

        //    // Load user data from file
        //    User? user = LoadUser(currentUser.Username);
        //    if (user == null)
        //        return false;

        //    // If username changes, delete the old file and update the username property.
        //    if (!currentUser.Username.Equals(newUsername, StringComparison.OrdinalIgnoreCase))
        //    {
        //        // Ensure the old file is deleted before updating
        //        bool deleteSuccess = DeleteUser(currentUser.Username);
        //        if (!deleteSuccess)
        //        {
        //            return false;  // If deleting the old user file failed, return false
        //        }

        //        // Update the user's username
        //        user.Username = newUsername;
        //    }

        //    // Update password with new hash
        //    if (!string.IsNullOrEmpty(newPassword))
        //    {
        //        user.Password = HashPassword(newPassword);  // Update password only if a new one is provided
        //    }

        //    // Save the updated user data
        //    SaveUser(user);

        //    // Now update the _currentUser in memory (the logged-in user) - update from the form layer
        //    currentUser.Username = newUsername;  // Update the logged-in user with the new username

        //    return true;
        //}

        public static bool UpdateUser(User currentUser, string newUsername, string newPassword)
        {
            // Check if new username is already taken (if changed)
            // Ensure new username is not taken
            if (!currentUser.Username.Equals(newUsername, StringComparison.OrdinalIgnoreCase) && LoadUser(newUsername) != null)
            {
                return false;
            }

            // Load user data
            User? user = LoadUser(currentUser.Username);
            if (user == null)
            {
                return false;
            }

            // If username changes, attempt to delete old user file
            if (!currentUser.Username.Equals(newUsername, StringComparison.OrdinalIgnoreCase))
            {
                bool deleteSuccess = DeleteUser(currentUser.Username);
                if (!deleteSuccess)
                {
                    return false;
                }
                user.Username = newUsername;
            }

            // Update password only if a new one is provided
            if (!string.IsNullOrEmpty(newPassword))
            {
                user.Password = HashPassword(newPassword);
            }

            // Save the updated user data
            SaveUser(user);

            // Now update the _currentUser in memory (the logged-in user) - update from the form layer
            currentUser.Username = user.Username;  // Update the logged-in user with the new username
            currentUser.Password = user.Password;  // Update the password in memory

            return true;
        }





        //// Delete a user's account
        //public static bool DeleteUser(string username)
        //{
        //    string userFilePath = Path.Combine(usersDirectory, $"{username}.json");
        //    if (File.Exists(userFilePath))
        //    {
        //        File.Delete(userFilePath);
        //        return true;
        //    }
        //    return false;
        //}

        public static bool DeleteUser(string username)
        {
            string userFilePath = Path.Combine(usersDirectory, $"{username}.json");

            if (File.Exists(userFilePath))
            {
                try
                {
                    File.Delete(userFilePath);
                    return true;
                }
                catch (Exception ex)
                {
                    // Log the error if the file cannot be deleted.
                    Logger.Log($"Error deleting user file {userFilePath}: {ex.Message}", "ERROR");
                    return false;
                }
            }

            return false;
        }


        public static void SaveUser(User user)
        {
            Directory.CreateDirectory(usersDirectory);
            string userFilePath = Path.Combine(usersDirectory, $"{user.Username}.json");
            string json = JsonSerializer.Serialize(user, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(userFilePath, json);
        }

        public static User? LoadUser(string username)
        {
            string userFilePath = Path.Combine(usersDirectory, $"{username}.json");
            if (!File.Exists(userFilePath))
                return null;

            string json = File.ReadAllText(userFilePath);
            return JsonSerializer.Deserialize<User>(json);
        }


        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static User? GetUser(string username, string password)
        {
            User? user = LoadUser(username);
            return (user != null && user.Password == HashPassword(password)) ? user : null;
        }

    }
}
