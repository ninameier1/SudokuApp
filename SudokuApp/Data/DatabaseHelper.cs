using System;
using System.Data.Odbc;
using System.Collections.Generic;
using SudokuApp.Models;


namespace SudokuApp.Data
{
    public class DatabaseHelper
    {
        private const string ConnectionString = "DSN=SudokuDB;Uid=root;Pwd=yourpassword;";

        // 🟢 Methode om alle spellen op te halen
        public static List<SudokuGame> GetAllGames()
        {
            List<SudokuGame> games = new List<SudokuGame>();

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM SudokuGame";

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    using (OdbcDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            games.Add(new SudokuGame
                            {
                                ID = reader.GetInt32(0),
                                PlayerName = reader.GetString(1),
                                GridState = reader.GetString(2),
                                TimeElapsed = reader.GetInt32(3)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Fout bij ophalen: {ex.Message}");
                }
            }
            return games;
        }

        // 🟢 Methode om een spel toe te voegen
        public static void InsertGame(SudokuGame game)
        {
            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO SudokuGame (PlayerName, GridState, TimeElapsed) VALUES (?, ?, ?)";

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PlayerName", game.PlayerName);
                        command.Parameters.AddWithValue("@GridState", game.GridState);
                        command.Parameters.AddWithValue("@TimeElapsed", game.TimeElapsed);

                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"✅ {rowsAffected} record toegevoegd!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Fout bij invoegen: {ex.Message}");
                }
            }
        }

        // 🟢 Methode om een spel te updaten
        public static void UpdateGame(int id, int newTimeElapsed)
        {
            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE SudokuGame SET TimeElapsed = ? WHERE ID = ?";

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TimeElapsed", newTimeElapsed);
                        command.Parameters.AddWithValue("@ID", id);

                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"✅ {rowsAffected} record geüpdatet!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Fout bij updaten: {ex.Message}");
                }
            }
        }

        // 🟢 Methode om een spel te verwijderen
        public static void DeleteGame(int id)
        {
            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM SudokuGame WHERE ID = ?";

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"✅ {rowsAffected} record verwijderd!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Fout bij verwijderen: {ex.Message}");
                }
            }
        }
    }
}
