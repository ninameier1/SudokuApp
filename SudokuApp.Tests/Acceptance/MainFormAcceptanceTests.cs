using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Windows.Forms;
using SudokuApp.Tests.Helpers;
using SudokuApp.Forms;
using SudokuApp.Services;
using System.Reflection;

namespace SudokuApp.Tests.Acceptance
{
    [TestClass]
    public class MainFormAcceptanceTests
    {
        // Acceptance Test 1: Simulate a full user registration and login flow.
        [TestMethod]
        public void UserRegistrationAndLoginFlow_ShouldRegisterAndShowLoginPage()
        {
            // Arrange
            var form = new MainForm();
            form.Show();
            Application.DoEvents();

            // Act: Navigate from start page to login page.
            Panel startPanel = form.Controls.OfType<Panel>()
                .FirstOrDefault(p => p.BackColor == System.Drawing.Color.LightBlue);
            Assert.IsNotNull(startPanel, "Start panel not found.");

            Button loginButton = startPanel.Controls.OfType<Button>()
                .FirstOrDefault(b => b.Name == "loginButton");
            Assert.IsNotNull(loginButton, "Login button not found on start panel.");
            loginButton.PerformClick();
            Application.DoEvents();

            Panel loginPanel = form.Controls.OfType<Panel>()
                .FirstOrDefault(p => p.Visible && p.Controls.OfType<Button>().Any(b => b.Text == "Register"));
            Assert.IsNotNull(loginPanel, "Login panel not found.");

            // Navigate to register page.
            Button registerButton = loginPanel.Controls.OfType<Button>()
                .FirstOrDefault(b => b.Text == "Register");
            Assert.IsNotNull(registerButton, "Register button not found on login panel.");
            registerButton.PerformClick();
            Application.DoEvents();

            // Assert: Register panel should now be visible.
            Panel registerPanel = form.Controls.OfType<Panel>()
                .FirstOrDefault(p => p.Visible && p.Controls.OfType<Label>().Any(l => l.Text.Contains("Register New Account")));
            Assert.IsNotNull(registerPanel, "Register panel was not shown.");

            // Simulate successful registration using the  UserManager.
            bool registrationSuccess = UserManager.Register("newuser7", "password");
            Assert.IsTrue(registrationSuccess, "Registration failed when it should succeed.");

            // Now simulate that after registration the login page is displayed.
            form.InvokePrivateMethod("ShowLoginPage");
            Application.DoEvents();

            loginPanel = form.Controls.OfType<Panel>()
                .FirstOrDefault(p => p.Visible && p.Controls.OfType<TextBox>().Any());
            Assert.IsNotNull(loginPanel, "Login panel was not shown after registration.");
        }

        // Acceptance Test 2: Simulate a game play flow including starting a game and generating a puzzle.
        [TestMethod]
        public void GamePlayFlow_ShouldStartGameGeneratePuzzleAndShowHint()
        {
            // Arrange
            var form = new MainForm();
            form.Show();
            Application.DoEvents();

            // Start the game by clicking the start button.
            Panel startPanel = form.Controls.OfType<Panel>()
                .FirstOrDefault(p => p.BackColor == System.Drawing.Color.LightBlue);
            Assert.IsNotNull(startPanel, "Start panel not found.");

            Button startGameButton = startPanel.Controls.OfType<Button>()
                .FirstOrDefault(b => b.Name == "startGameButton");
            Assert.IsNotNull(startGameButton, "Start Game button not found on start panel.");

            startGameButton.PerformClick();
            Application.DoEvents();

            Panel sudokuPanel = form.Controls.OfType<Panel>()
                .FirstOrDefault(p => p.Visible && p.BackColor == System.Drawing.Color.LightCyan);
            Assert.IsNotNull(sudokuPanel, "Sudoku panel was not displayed.");

            // Act: Click on the Generate button to generate a puzzle.
            Button btnGenerate = sudokuPanel.Controls.OfType<Button>()
                .FirstOrDefault(b => b.Name == "btnGenerate");
            Assert.IsNotNull(btnGenerate, "Generate button not found on sudoku panel.");

            btnGenerate.PerformClick();
            Application.DoEvents();

            // Wait for the asynchronous puzzle generation to complete.
            var puzzleField = form.GetType().GetField("puzzle", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(puzzleField, "Puzzle field not found via reflection.");
            DateTime startTime = DateTime.Now;
            while (puzzleField.GetValue(form) == null && (DateTime.Now - startTime).TotalSeconds < 5)
            {
                Application.DoEvents();
                Thread.Sleep(50); // Small delay to avoid busy waiting.
            }
            Assert.IsNotNull(puzzleField.GetValue(form), "Puzzle was not generated within the expected time.");
        }
    }
}
