using System;
using System.Windows.Forms;
using SudokuApp.Data;
using SudokuApp.Forms;



namespace SudokuApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(""));
            //InsertData("Cecilia", "123456789...", 120);




            {
                Console.WriteLine("🎲 Sudoku Database Test 🎲");

                // 🔹 Gegevens toevoegen
                SudokuGame newGame = new SudokuGame
                {
                    PlayerName = "Cecilia",
                    GridState = "123456789...",
                    TimeElapsed = 120
                };
                DatabaseHelper.InsertGame(newGame);


                // 🔹 Gegevens ophalen en weergeven
                var games = DatabaseHelper.GetAllGames();
                Console.WriteLine("\n📌 Sudoku spellen in de database:");
                foreach (var game in games)
                {
                    Console.WriteLine($"ID: {game.ID}, Speler: {game.PlayerName}, Tijd: {game.TimeElapsed} sec");
                }

                // 🔹 Gegevens updaten
                if (games.Count > 0)
                {
                    DatabaseHelper.UpdateGame(games[0].ID, 300);
                    Console.WriteLine($"✅ Tijd aangepast voor speler {games[0].PlayerName}");
                }

                // 🔹 Gegevens verwijderen
                if (games.Count > 1)
                {
                    DatabaseHelper.DeleteGame(games[1].ID);
                    Console.WriteLine($"❌ Spel van {games[1].PlayerName} verwijderd.");
                }
            }
        }

    }
}

//using System;
//using System.Windows.Forms;
//using SudokuApp.Data;
//using SudokuApp.Forms;
//using SudokuApp.Models;

//namespace SudokuApp
//{
//    static class Program
//    {
//        [STAThread]
//        static void Main()
//        {
//            // Initialisatie van Windows Forms
//            Application.EnableVisualStyles();
//            Application.SetCompatibleTextRenderingDefault(false);

//            // Test databaseverbinding (alleen tijdens ontwikkeling)
//            TestDatabaseOperations();

//            // Start de hoofdapplicatie
//            Application.Run(new MainForm(""));
//        }

//        static void TestDatabaseOperations()
//        {
//#if DEBUG
//            Console.WriteLine("🎲 Sudoku Database Test 🎲");

//            try
//            {
//                // 🔹 Gegevens toevoegen
//                SudokuGame newGame = new SudokuGame
//                {
//                    PlayerName = "Cecilia",
//                    GridState = "123456789...",
//                    TimeElapsed = 120
//                };
//                DatabaseHelper.InsertGame(newGame);

//                // 🔹 Gegevens ophalen en weergeven
//                var games = DatabaseHelper.GetAllGames();
//                Console.WriteLine("\n📌 Sudoku spellen in de database:");
//                foreach (var game in games)
//                {
//                    Console.WriteLine($"ID: {game.ID}, Speler: {game.PlayerName}, Tijd: {game.TimeElapsed} sec");
//                }

//                // 🔹 Gegevens updaten
//                if (games.Count > 0)
//                {
//                    DatabaseHelper.UpdateGame(games[0].ID, 300);
//                    Console.WriteLine($"✅ Tijd aangepast voor speler {games[0].PlayerName}");
//                }

//                // 🔹 Gegevens verwijderen
//                if (games.Count > 1)
//                {
//                    DatabaseHelper.DeleteGame(games[1].ID);
//                    Console.WriteLine($"❌ Spel van {games[1].PlayerName} verwijderd.");
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"❌ Databasefout: {ex.Message}");
//            }
//#endif
//        }
//    }
//}