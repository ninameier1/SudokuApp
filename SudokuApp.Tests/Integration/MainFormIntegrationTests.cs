using Microsoft.VisualStudio.TestTools.UnitTesting;
using SudokuApp.Forms;
using System.Linq;
using System.Windows.Forms;
using SudokuApp.Tests.Helpers;

namespace SudokuApp.Tests.Integration
{
    [TestClass]
    public class MainFormIntegrationTests
    {
        // Integration Test 1: Simulate clicking the Login/Register button to ensure the login page is shown.
        [TestMethod]
        public void LoginButton_Click_ShouldShowLoginPanel()
        {
            // Arrange
            var form = new MainForm();
            form.Show(); // Ensure the form is displayed.
            Application.DoEvents();

            Panel startPanel = form.Controls.OfType<Panel>()
                .FirstOrDefault(p => p.BackColor == System.Drawing.Color.LightBlue);
            Assert.IsNotNull(startPanel, "Start panel not found.");

            Button loginButton = startPanel.Controls.OfType<Button>()
                .FirstOrDefault(b => b.Name == "loginButton");
            Assert.IsNotNull(loginButton, "Login button not found on start panel.");

            // Act: Simulate the button click.
            loginButton.PerformClick();
            Application.DoEvents();

            // Assert: Verify that loginPanel is visible and startPanel is hidden.
            Panel loginPanel = form.Controls.OfType<Panel>()
                .FirstOrDefault(p => p.Visible && p.Controls.OfType<TextBox>().Any());
            Assert.IsNotNull(loginPanel, "Login panel was not shown after login button click.");
            Assert.IsFalse(startPanel.Visible, "Start panel should be hidden after login button click.");
        }

        // Integration Test 2: Simulate clicking the Start Game button to ensure the sudoku page is shown.
        [TestMethod]
        public void StartGameButton_Click_ShouldShowSudokuPanel()
        {
            // Arrange
            var form = new MainForm();
            form.Show();
            Application.DoEvents();

            Panel startPanel = form.Controls.OfType<Panel>()
                .FirstOrDefault(p => p.BackColor == System.Drawing.Color.LightBlue);
            Assert.IsNotNull(startPanel, "Start panel not found.");

            Button startGameButton = startPanel.Controls.OfType<Button>()
                .FirstOrDefault(b => b.Name == "startGameButton");
            Assert.IsNotNull(startGameButton, "Start Game button not found on start panel.");

            // Act: Simulate the button click.
            startGameButton.PerformClick();
            Application.DoEvents();

            // Assert: Verify that sudokuPanel is visible and startPanel is hidden.
            Panel sudokuPanel = form.Controls.OfType<Panel>()
                .FirstOrDefault(p => p.Visible && p.BackColor == System.Drawing.Color.LightCyan);
            Assert.IsNotNull(sudokuPanel, "Sudoku panel was not shown after clicking start game.");
            Assert.IsFalse(startPanel.Visible, "Start panel should be hidden after clicking start game.");
        }
    }
}
