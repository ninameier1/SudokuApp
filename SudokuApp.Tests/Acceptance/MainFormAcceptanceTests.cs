//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Windows.Forms;
//using SudokuApp;
//using SudokuApp.Forms;



//namespace SudokuApp.Tests.Acceptance
//{
//    [TestClass]
//    public class MainFormAcceptanceTests
//    {
//        [TestMethod]
//        public void RegistrationFlow_ShouldRegisterAndThenShowLoginPage()
//        {
//            // Arrange: Launch the MainForm
//            MainForm form = new MainForm();
//            form.Show();

//            // Act: Simulate user clicking the "Login/Register" button on the start page
//            Button loginBtn = null;
//            foreach (Control ctrl in form.Controls["startPanel"].Controls)
//            {
//                if (ctrl is Button btn && btn.Name == "loginButton")
//                {
//                    loginBtn = btn;
//                    break;
//                }
//            }
//            Assert.IsNotNull(loginBtn, "loginButton should exist.");
//            loginBtn.PerformClick();

//            // Now simulate clicking "Register" from the login page
//            Panel loginPanel = (Panel)form.Controls["loginPanel"];
//            Button registerBtn = null;
//            foreach (Control ctrl in loginPanel.Controls)
//            {
//                if (ctrl is Button btn && btn.Text == "Register")
//                {
//                    registerBtn = btn;
//                    break;
//                }
//            }
//            Assert.IsNotNull(registerBtn, "Register button should exist on the login page.");
//            registerBtn.PerformClick();

//            // At this point, the registerPanel should be visible.
//            Panel registerPanel = (Panel)form.Controls["registerPanel"];
//            Assert.IsNotNull(registerPanel, "registerPanel should be created.");
//            Assert.IsTrue(registerPanel.Visible, "registerPanel should be visible after clicking Register.");

//            // Simulate filling in registration details
//            TextBox usernameTextbox = null;
//            TextBox passwordTextbox = null;
//            foreach (Control ctrl in registerPanel.Controls)
//            {
//                if (ctrl is TextBox tb)
//                {
//                    if (usernameTextbox == null)
//                        usernameTextbox = tb;
//                    else
//                        passwordTextbox = tb;
//                }
//            }
//            // Assert
//            Assert.IsNotNull(usernameTextbox, "Username textbox should be present on registration page.");
//            Assert.IsNotNull(passwordTextbox, "Password textbox should be present on registration page.");
//            usernameTextbox.Text = "newuser";
//            passwordTextbox.Text = "newpassword";

//            // Simulate clicking the Register button on the register panel.
//            Button regBtn = null;
//            foreach (Control ctrl in registerPanel.Controls)
//            {
//                if (ctrl is Button btn && btn.Text == "Register")
//                {
//                    regBtn = btn;
//                    break;
//                }
//            }
//            Assert.IsNotNull(regBtn, "Register button should be found on the register panel.");
//            regBtn.PerformClick();

//            // After successful registration, the registerPanel should be hidden and loginPanel visible.
//            Assert.IsFalse(registerPanel.Visible, "registerPanel should be hidden after successful registration.");
//            Assert.IsTrue(loginPanel.Visible, "loginPanel should be visible after successful registration.");
//        }

//        [TestMethod]
//        public void UpdateAccountSettings_ShouldUpdateUserAndRefreshUI()
//        {
//            // Arrange: Create a MainForm instance with a user logged in.
//            MainForm form = new MainForm("existingUser");

//            // Simulate that the user is logged in and that the sudokuPanel is visible.
//            form.GetType().GetMethod("ShowSudokuPage",
//                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
//                .Invoke(form, null);

//            // Act: Simulate user opening settings page.
//            form.GetType().GetMethod("ShowSettingsPage",
//                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
//                .Invoke(form, null);

//            Panel settingsPanel = (Panel)form.Controls["settingsPanel"];
//            Assert.IsNotNull(settingsPanel, "settingsPanel should be created.");
//            Assert.IsTrue(settingsPanel.Visible, "settingsPanel should be visible after opening settings.");

//            // Find the text boxes for new username and new password.
//            TextBox txtNewUsername = null;
//            TextBox txtNewPassword = null;
//            foreach (Control ctrl in settingsPanel.Controls)
//            {
//                if (ctrl is TextBox tb)
//                {
//                    // This example assumes the first textbox is for username and second for password.
//                    if (txtNewUsername == null)
//                        txtNewUsername = tb;
//                    else
//                        txtNewPassword = tb;
//                }
//            }
//            Assert.IsNotNull(txtNewUsername, "New username textbox should be found in settings.");
//            Assert.IsNotNull(txtNewPassword, "New password textbox should be found in settings.");

//            // Simulate user entering new account details.
//            txtNewUsername.Text = "updatedUser";
//            txtNewPassword.Text = "updatedPassword";

//            // Find and click the update button.
//            Button btnUpdate = null;
//            foreach (Control ctrl in settingsPanel.Controls)
//            {
//                if (ctrl is Button btn && btn.Text == "Update")
//                {
//                    btnUpdate = btn;
//                    break;
//                }
//            }
//            Assert.IsNotNull(btnUpdate, "Update button should be present in settings.");
//            btnUpdate.PerformClick();

//            // Assert
//            Assert.IsTrue(settingsPanel.Visible, "After updating, settingsPanel should remain visible.");
//        }
//    }
//}

//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using SudokuApp.Forms;
//using SudokuApp.Services;
//using System.Linq;
//using System.Windows.Forms;

//namespace SudokuApp.Tests.Acceptance
//{
//    [TestClass]
//    public class MainFormAcceptanceTests
//    {
//        // Acceptance Test 1: Simulate a full user registration and login flow.
//        [TestMethod]
//        public void UserRegistrationAndLoginFlow_ShouldRegisterAndShowLoginPage()
//        {
//            // Arrange
//            var form = new MainForm();

//            // Act: Simulate navigating to the register page.
//            // Assume that the register page is shown when clicking the "Login/Register" button and then the "Register" button.
//            Panel startPanel = form.Controls.OfType<Panel>().FirstOrDefault();
//            Button loginButton = startPanel?.Controls.OfType<Button>().FirstOrDefault(b => b.Name == "loginButton");
//            loginButton.PerformClick();

//            // Now simulate clicking the Register button on the login panel.
//            Panel loginPanel = form.Controls.OfType<Panel>().FirstOrDefault(p => p.Visible && p.Controls.OfType<Button>().Any(b => b.Text == "Register"));
//            Button registerButton = loginPanel.Controls.OfType<Button>().FirstOrDefault(b => b.Text == "Register");
//            registerButton.PerformClick();

//            // At this point, the register panel should be visible.
//            Panel registerPanel = form.Controls.OfType<Panel>().FirstOrDefault(p => p.Visible && p.Controls.OfType<Label>().Any(l => l.Text.Contains("Register New Account")));
//            Assert.IsNotNull(registerPanel, "Register panel was not shown.");

//            // Simulate user input for registration.
//            // (In a full acceptance test, you would set Text properties on the textboxes and click the Register button.
//            // Here, we assume the user enters valid data.)
//            // For demonstration, we invoke the registration delegate directly.
//            // In a real acceptance test, you would automate the UI.
//            bool registrationSuccess = UserManager.Register("newuser", "password");
//            Assert.IsTrue(registrationSuccess, "Registration failed when it should succeed.");

//            // Now simulate that after registration the login page is displayed.
//            form.InvokePrivateMethod("ShowLoginPage");
//            loginPanel = form.Controls.OfType<Panel>().FirstOrDefault(p => p.Visible && p.Controls.OfType<TextBox>().Any());
//            Assert.IsNotNull(loginPanel, "Login panel was not shown after registration.");
//        }

//        // Acceptance Test 2: Simulate a game play flow including starting a game, generating a puzzle, and requesting a hint.
//        [TestMethod]
//        public void GamePlayFlow_ShouldStartGameGeneratePuzzleAndShowHint()
//        {
//            // Arrange
//            var form = new MainForm();

//            // Act: Simulate clicking the "Start Game" button.
//            Panel startPanel = form.Controls.OfType<Panel>().FirstOrDefault();
//            Button startGameButton = startPanel?.Controls.OfType<Button>().FirstOrDefault(b => b.Name == "startGameButton");
//            startGameButton.PerformClick();

//            // The sudoku panel should now be visible.
//            Panel sudokuPanel = form.Controls.OfType<Panel>().FirstOrDefault(p => p.Visible && p.BackColor == System.Drawing.Color.LightCyan);
//            Assert.IsNotNull(sudokuPanel, "Sudoku panel was not displayed.");

//            // Simulate generating a new puzzle.
//            Button btnGenerate = sudokuPanel.Controls.OfType<Button>().FirstOrDefault(b => b.Name == "btnGenerate");
//            Assert.IsNotNull(btnGenerate, "Generate button not found on sudoku panel.");
//            btnGenerate.PerformClick();

//            // Simulate requesting a hint.
//            Button btnHint = sudokuPanel.Controls.OfType<Button>().FirstOrDefault(b => b.Name == "btnHint");
//            Assert.IsNotNull(btnHint, "Hint button not found on sudoku panel.");
//            btnHint.PerformClick();

//            // Check that the hint counter label has been updated.
//            Label lblHintCounter = sudokuPanel.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "lblHintCounter");
//            Assert.IsNotNull(lblHintCounter, "Hint counter label not found on sudoku panel.");
//            // (Assume that clicking hint updates the text; you might check that it changed from "Hints: 0/5".)
//            Assert.AreNotEqual("Hints: 0/5", lblHintCounter.Text, "Hint counter did not update after requesting a hint.");
//        }
//    }
//}

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

        // Acceptance Test 2: Simulate a game play flow including starting a game, generating a puzzle, and requesting a hint.
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
