using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using SudokuApp.Models;

namespace SudokuApp.Services
{
    public class UserService
    {
        private const string ConnectionString = "DSN=SudokuDB;Uid=root;Pwd=yourpassword;";

        // Voeg nieuwe gebruiker toe (met parameterized query)
        public void AddUser(User user)
        {
            using (var connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();
                var command = new OdbcCommand(
                    "INSERT INTO Users (Username, Password, TotalMinutesPlayed, PuzzlesSolved, LeaderboardRank, SuccessRate) " +
                    "VALUES (?, ?, ?, ?, ?, ?)",
                    connection);

                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@TotalMinutesPlayed", user.TotalMinutesPlayed);
                command.Parameters.AddWithValue("@PuzzlesSolved", user.PuzzlesSolved);
                command.Parameters.AddWithValue("@LeaderboardRank", user.LeaderboardRank);
                command.Parameters.AddWithValue("@SuccessRate", user.SuccessRate);

                command.ExecuteNonQuery();
            }
        }

        // Haal alle gebruikers op met LINQ filtering
        public List<User> GetUsersByMinPlaytime(int minMinutes)
        {
            var allUsers = GetAllUsers();

            // LINQ query (schoolvereiste)
            return allUsers
                .Where(u => u.TotalMinutesPlayed >= minMinutes)
                .OrderByDescending(u => u.SuccessRate)
                .ToList();
        }

        // Update gebruikersstatistieken
        public void UpdateUserStats(int userId, int minutesPlayed, bool puzzleSolved)
        {
            var user = GetUserById(userId);
            if (user == null) return;

            user.TotalMinutesPlayed += minutesPlayed;
            if (puzzleSolved) user.PuzzlesSolved++;

            // LINQ berekening voor success rate (schoolvereiste)
            var allUsers = GetAllUsers();
            user.SuccessRate = allUsers.Any() ?
                (float)user.PuzzlesSolved / allUsers.Average(u => u.PuzzlesSolved) : 0;

            UpdateUserInDatabase(user);
        }

        // Helper methodes
        private List<User> GetAllUsers()
        {
            var users = new List<User>();

            using (var connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();
                var command = new OdbcCommand("SELECT * FROM Users", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Username = reader["Username"].ToString(),
                            Password = reader["Password"].ToString(),
                            TotalMinutesPlayed = Convert.ToInt32(reader["TotalMinutesPlayed"]),
                            PuzzlesSolved = Convert.ToInt32(reader["PuzzlesSolved"]),
                            LeaderboardRank = Convert.ToInt32(reader["LeaderboardRank"]),
                            SuccessRate = Convert.ToSingle(reader["SuccessRate"])
                        });
                    }
                }
            }
            return users;
        }

        private User? GetUserById(int id)
        {
            // LINQ FirstOrDefault (schoolvereiste)
            return GetAllUsers().FirstOrDefault(u => u.Id == id);
        }

        private void UpdateUserInDatabase(User user)
        {
            using (var connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();
                var command = new OdbcCommand(
                    "UPDATE Users SET " +
                    "Username = ?, Password = ?, TotalMinutesPlayed = ?, " +
                    "PuzzlesSolved = ?, LeaderboardRank = ?, SuccessRate = ? " +
                    "WHERE Id = ?",
                    connection);

                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@TotalMinutesPlayed", user.TotalMinutesPlayed);
                command.Parameters.AddWithValue("@PuzzlesSolved", user.PuzzlesSolved);
                command.Parameters.AddWithValue("@LeaderboardRank", user.LeaderboardRank);
                command.Parameters.AddWithValue("@SuccessRate", user.SuccessRate);
                command.Parameters.AddWithValue("@Id", user.Id);

                command.ExecuteNonQuery();
            }
        }
    }
}
