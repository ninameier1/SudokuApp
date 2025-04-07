using Microsoft.VisualStudio.TestTools.UnitTesting;
using SudokuApp.Forms;
using System.Linq;
using System.Windows.Forms;
using SudokuApp.Tests.Helpers;

namespace SudokuApp.Tests.Unit
{
    [TestClass]
    public class MainFormUnitTests
    {
        // Unit Test 1: Verify that InitializeStartPage creates and adds the startPanel with a Start Game button.
        [TestMethod]
        public void InitializeStartPage_ShouldAddStartPanelWithStartGameButton()
        {
            // Arrange
            var form = new MainForm();

            // Act: The default constructor calls InitializeStartPage, so we assume it has run.
            // We try to find the startPanel by checking the Controls collection.
            Panel startPanel = form.Controls.OfType<Panel>().FirstOrDefault(p => p.BackColor == System.Drawing.Color.LightBlue);

            // Assert
            Assert.IsNotNull(startPanel, "The start panel was not created.");
            // Check that a button with name "startGameButton" is present.
            Button startGameButton = startPanel.Controls.OfType<Button>().FirstOrDefault(b => b.Name == "startGameButton");
            Assert.IsNotNull(startGameButton, "The start game button was not added to the start panel.");
        }

        // Unit Test 2: Verify that InitializeLoginPage adds the loginPanel and its controls correctly.
        [TestMethod]
        public void InitializeLoginPage_ShouldAddLoginControls()
        {
            // Arrange
            var form = new MainForm();
            // Act: Call the method to initialize login page.
            // For testing, we assume the method is accessible.
            form.InvokePrivateMethod("InitializeLoginPage");

            // Retrieve the login panel
            Panel loginPanel = form.Controls.OfType<Panel>().FirstOrDefault(p => p.BackColor == System.Drawing.Color.LightBlue && p.Controls.OfType<TextBox>().Any());
            Assert.IsNotNull(loginPanel, "The login panel was not created.");

            // Check that expected controls (labels, textboxes, and buttons) exist.
            Label usernameLabel = loginPanel.Controls.OfType<Label>().FirstOrDefault(l => l.Text.Contains("Username"));
            Assert.IsNotNull(usernameLabel, "Username label is missing in login panel.");
            TextBox usernameTextbox = loginPanel.Controls.OfType<TextBox>().FirstOrDefault();
            Assert.IsNotNull(usernameTextbox, "Username textbox is missing in login panel.");
            Button loginButton = loginPanel.Controls.OfType<Button>().FirstOrDefault(b => b.Text == "Login");
            Assert.IsNotNull(loginButton, "Login button is missing in login panel.");
        }
    }

    // Helper extension to invoke private methods for testing purposes.
    //public static class ReflectionExtensions
    //{
    //    public static void InvokePrivateMethod(this object obj, string methodName, params object[] parameters)
    //    {
    //        var type = obj.GetType();
    //        var method = type.GetMethod(methodName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
    //        method?.Invoke(obj, parameters);
    //    }
    //}
}
