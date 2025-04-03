using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using SudokuApp.Models;
using SudokuApp.Services;
using SudokuApp.Utils;
using SudokuApp.CustomControls;
using System.Text.Json;

namespace SudokuApp.Forms
{
    public partial class MainForm : Form
    {
        private SudokuPuzzle puzzle;
        private string _username;
        private Panel startPanel;
        private Panel sudokuPanel;
        private Panel loginPanel;
        private Panel settingsPanel;
        private Panel registerPanel; // New registration panel
        private SudokuGrid sudokuGrid;
        private Button btnReset;
        private Button btnHint;
        private int gridSize = 9;
        private CancellationTokenSource _cancellationTokenSource = null;
        private int hintCount = 0;
        private const int maxHints = 5;
        private Label lblHintCounter;
        private User _currentUser;
        private System.Windows.Forms.Timer playTimer;
        private TimeSpan playTime = TimeSpan.Zero;
        private Label lblPlayTime;
        private ListView lvLeaderboard;
        private ComboBox cmbSize;


        public MainForm()
        {
            InitializeComponent();
            InitializeStartPage();
            this.Resize += MainForm_Resize; // Register for the Resize event
        }

        public MainForm(string username)
        {
            InitializeComponent();
            _username = username;
            InitializeStartPage();
        }

        private void InitializeStartPage()
        {
            // Create the start page panel
            startPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightBlue
            };
            this.Controls.Add(startPanel);

            // Add a button to start the game
            Button startGameButton = new Button
            {
                Text = "Start Sudoku",
                Width = 150,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Lavender,

                Left = (this.Width - 150) / 2,
                Top = (this.Height - 50) / 2,
                Name = "startGameButton"
            };
            startGameButton.FlatAppearance.BorderColor = Color.Plum; // Set the border color to Plum
            startGameButton.FlatAppearance.BorderSize = 2; 
            startGameButton.Click += StartGameButton_Click;
            startPanel.Controls.Add(startGameButton);

            // Add a login button
            Button loginButton = new Button
            {
                Text = "Login/Register",
                Width = 150,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Lavender,
                Left = (this.Width - 150) / 2,
                Top = startGameButton.Bottom + 10,
                Name = "loginButton"
            };
            loginButton.FlatAppearance.BorderColor = Color.Plum; 
            loginButton.FlatAppearance.BorderSize = 2; 
            loginButton.Click += LoginButton_Click;
            startPanel.Controls.Add(loginButton);
        }



        private void LoginButton_Click(object sender, EventArgs e)
        {
            ShowLoginPage();
        }

        private void ShowLoginPage()
        {
            // Hide other panels
            if (startPanel != null) startPanel.Visible = false;
            if (sudokuPanel != null) sudokuPanel.Visible = false;
            if (registerPanel != null) registerPanel.Visible = false;
            if (settingsPanel != null) settingsPanel.Visible = false;

            // Initialize and show the login page if not already created
            if (loginPanel == null)
            {
                InitializeLoginPage();
            }
            loginPanel.Visible = true;
        }


        private void InitializeLoginPage()
        {
            loginPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightBlue
            };
            this.Controls.Add(loginPanel);

            // Add login controls
            Label usernameLabel = new Label
            {
                Text = "Username:",
                Left = (this.Width - 200) / 2,
                Top = (this.Height - 100) / 2,
            };
            loginPanel.Controls.Add(usernameLabel);

            TextBox usernameTextbox = new TextBox
            {
                Width = 155,
                Left = (this.Width - 200) / 2,
                Top = usernameLabel.Bottom + 1,
            };
            loginPanel.Controls.Add(usernameTextbox);

            Label passwordLabel = new Label
            {
                Text = "Password:",
                Left = (this.Width - 200) / 2,
                Top = usernameTextbox.Bottom + 10,
            };
            loginPanel.Controls.Add(passwordLabel);

            TextBox passwordTextbox = new TextBox
            {
                Width = 155,
                Left = (this.Width - 200) / 2,
                Top = passwordLabel.Bottom + 1,
                PasswordChar = '*'
            };
            loginPanel.Controls.Add(passwordTextbox);

            Button loginButton = new Button
            {
                Text = "Login",
                Width = 75,
                BackColor = Color.Lavender,
                FlatStyle = FlatStyle.Flat,
                Left = (this.Width - 200) / 2,
                Top = passwordTextbox.Bottom + 10,
            };
            loginButton.Click += (s, e) => LoginUser(usernameTextbox.Text, passwordTextbox.Text);
            loginPanel.Controls.Add(loginButton);

            // Add a Register button that shows the registration page
            Button registerButton = new Button
            {
                Text = "Register",
                Width = 75,
                BackColor = Color.Lavender,
                FlatStyle = FlatStyle.Flat,
                Left = loginButton.Right + 5,
                Top = passwordTextbox.Bottom + 10,
            };
            registerButton.Click += (s, e) => ShowRegisterPage();
            loginPanel.Controls.Add(registerButton);

            Button cancelButton = new Button
            {
                Text = "Cancel",
                Width = 75,
                BackColor = Color.Lavender,
                FlatStyle = FlatStyle.Flat,
                Left = registerButton.Right + 5,
                Top = passwordTextbox.Bottom + 10,
            };
            cancelButton.Click += (s, e) => CancelLogin();
            loginPanel.Controls.Add(cancelButton);
        }

        private void ShowRegisterPage()
        {
            // Hide the login panel if it's visible
            if (loginPanel != null)
                loginPanel.Visible = false;

            // Initialize the register page if it hasn't been created
            if (registerPanel == null)
            {
                InitializeRegisterPage();
            }
            registerPanel.Visible = true;
        }

        private void InitializeRegisterPage()
        {
           registerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightBlue
            };
            this.Controls.Add(registerPanel);

            // Add registration controls
            Label regLabel = new Label
            {
                Text = "Register New Account",
                Font = new Font("Arial", 14, FontStyle.Bold),
                AutoSize = true,
                Left = (this.Width - 200) / 2,
                Top = (this.Height - 150) / 2 - 30
            };
            registerPanel.Controls.Add(regLabel);

            Label usernameLabel = new Label
            {
                Text = "Username:",
                Left = (this.Width - 200) / 2,
                Top = regLabel.Bottom + 10,
            };
            registerPanel.Controls.Add(usernameLabel);

            TextBox usernameTextbox = new TextBox
            {
                Width = 155,
                Left = (this.Width - 200) / 2,
                Top = usernameLabel.Bottom + 1,
            };
            registerPanel.Controls.Add(usernameTextbox);

            Label passwordLabel = new Label
            {
                Text = "Password:",
                Left = (this.Width - 200) / 2,
                Top = usernameTextbox.Bottom + 10,
            };
            registerPanel.Controls.Add(passwordLabel);

            TextBox passwordTextbox = new TextBox
            {
                Width = 155,
                Left = (this.Width - 200) / 2,
                Top = passwordLabel.Bottom + 1,
                PasswordChar = '*'
            };
            registerPanel.Controls.Add(passwordTextbox);

            Button registerButton = new Button
            {
                Text = "Register",
                Width = 75,
                BackColor = Color.Lavender,
                FlatStyle = FlatStyle.Flat,
                Left = (this.Width - 200) / 2,
                Top = passwordTextbox.Bottom + 10,
            };
            registerButton.Click += (s, e) =>
            {
                // Attempt to register the user
                if (UserManager.Register(usernameTextbox.Text, passwordTextbox.Text))
                {
                    MessageBox.Show("Registration successful! Please log in.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // After successful registration, hide the register panel and show the login page
                    registerPanel.Visible = false;
                    ShowLoginPage();
                }
                else
                {
                    MessageBox.Show("Username already taken. Please choose a different one.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            registerPanel.Controls.Add(registerButton);

            Button cancelButton = new Button
            {
                Text = "Cancel",
                Width = 75,
                BackColor = Color.Lavender,
                FlatStyle = FlatStyle.Flat,
                Left = registerButton.Right + 5,
                Top = passwordTextbox.Bottom + 10,
            };
            cancelButton.Click += (s, e) =>
            {
                // Hide registration panel and return to login page
                registerPanel.Visible = false;
                ShowLoginPage();
            };
            registerPanel.Controls.Add(cancelButton);
        }



        private void BtnGoToLogin_Click(object sender, EventArgs e)
        {
            // Hide Sudoku page
            if (sudokuPanel != null)
                sudokuPanel.Visible = false;

            // Show Login page
            ShowLoginPage();
            RefreshUI();
        }

        private void StartGameButton_Click(object sender, EventArgs e)
        {
            ShowSudokuPage();
            RefreshUI();
        }

        private void ShowSudokuPage()
        {
            // Hide the start page
            if (startPanel != null)
                startPanel.Visible = false;

            // Hide login and register panels if visible
            if (loginPanel != null) loginPanel.Visible = false;
            if (registerPanel != null) registerPanel.Visible = false;

            // Initialize and show the Sudoku game page if not already created
            if (sudokuPanel == null)
            {
                InitializeSudokuPage();
            }
            sudokuPanel.Visible = true;
            sudokuGrid.ClearBoard();
            RefreshUI();
        }

        private void InitializeSudokuPage()
        {
            sudokuPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightCyan
            };
            this.Controls.Add(sudokuPanel);

            // Initialize the SudokuGrid control and pass gridSize to it
            sudokuGrid = new SudokuGrid(gridSize)
            {
                Location = new Point(190, 10)
            };
            sudokuPanel.Controls.Add(sudokuGrid);


            // Create and configure the leaderboard ListView
            lvLeaderboard = new ListView
            {
                Name = "lvLeaderboard",
                View = View.Details,
                Width = 170,
                Height = 200,
                BackColor = Color.LightCyan,
                BorderStyle = BorderStyle.None,
                Location = new Point(10, 10)
            };
            lvLeaderboard.Columns.Add("User", 100);
            lvLeaderboard.Columns.Add("Points", 70);
            sudokuPanel.Controls.Add(lvLeaderboard);

            // ComboBox for puzzle size
            cmbSize = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.Lavender,
                Width = 100,
                Height = 30,
                Location = new Point(sudokuGrid.Left, sudokuGrid.Bottom + 10),
                Name = "cmbSize"
            };
            cmbSize.Items.AddRange(new object[] { "4x4", "9x9", "16x16" });
            cmbSize.SelectedIndex = 1; // Default to 9x9
            cmbSize.SelectedIndexChanged += (s, e) =>
            {
                string selected = cmbSize.SelectedItem.ToString();
                gridSize = selected == "4x4" ? 4 : selected == "9x9" ? 9 : 16;

                // Reset puzzle and update the grid size.
                sudokuGrid.UpdateGridSize(gridSize);
                ResetPuzzle();
            };
            sudokuPanel.Controls.Add(cmbSize);

            // Button for generating a new puzzle
            Button btnGenerate = new Button
            {
                Text = "Generate",
                Width = 120,
                Height = 30,
                Location = new Point(cmbSize.Right + 10, cmbSize.Top),
                BackColor = Color.Lavender,
                FlatStyle = FlatStyle.Flat,
                Name = "btnGenerate"
            };
            btnGenerate.FlatAppearance.BorderColor = Color.Plum; // Set the border color to Plum
            btnGenerate.FlatAppearance.BorderSize = 2;
            btnGenerate.Click += async (sender, e) => await btnGenerate_Click(sender, e);
            sudokuPanel.Controls.Add(btnGenerate);

            // Add reset button
            Button btnReset = new Button
            {
                Text = "Reset",
                Width = 100,
                Height = 30,
                Location = new Point(btnGenerate.Right + 10, btnGenerate.Top),
                BackColor = Color.Lavender,
                FlatStyle = FlatStyle.Flat,
                Name = "btnReset"
            };
            btnReset.FlatAppearance.BorderColor = Color.Plum; // Set the border color to Plum
            btnReset.FlatAppearance.BorderSize = 2;
            btnReset.Click += btnReset_Click;
            sudokuPanel.Controls.Add(btnReset);

            // Add hint button
            Button btnHint = new Button
            {
                Text = "Show Hint",
                Width = 100,
                Height = 30,
                Location = new Point(btnReset.Right + 10, sudokuGrid.Bottom + 10),
                BackColor = Color.Lavender,
                FlatStyle = FlatStyle.Flat,
                Name = "btnHint"
            };
            btnHint.FlatAppearance.BorderColor = Color.Plum; // Set the border color to Plum
            btnHint.FlatAppearance.BorderSize = 2;
            btnHint.Click += btnHint_Click;
            sudokuPanel.Controls.Add(btnHint);

            // Add hint counter
            lblHintCounter = new Label
            {
                Text = "Hints: 0/5",
                AutoSize = true,
                Left = btnHint.Left,
                Top = btnHint.Bottom + 2,
                Name = "lblHintCounter"
            };
            sudokuPanel.Controls.Add(lblHintCounter);

            // Button for solving the puzzle
            Button btnSolve = new Button
            {
                Text = "Solve",
                Width = 100,
                Height = 30,
                Location = new Point(sudokuGrid.Right - 100, btnHint.Top),
                BackColor = Color.Lavender,
                FlatStyle = FlatStyle.Flat,
                Name = "btnSolve"
            };
            btnSolve.FlatAppearance.BorderColor = Color.Plum; // Set the border color to Plum
            btnSolve.FlatAppearance.BorderSize = 2;
            btnSolve.Click += async (s, e) => await SolvePuzzleAsync();
            sudokuPanel.Controls.Add(btnSolve);

            // Button for saving the current game
            Button btnSave = new Button
            {
                Text = "Save Game",
                Width = 100,
                Height = 30,
                Location = new Point(btnSolve.Left, btnSolve.Bottom + 10),
                BackColor = Color.Lavender,
                FlatStyle = FlatStyle.Flat,
                Name = "btnSave"
            };
            btnSave.Click += BtnSave_Click;
            sudokuPanel.Controls.Add(btnSave);

            // Button for loading a saved game
            Button btnLoad = new Button
            {
                Text = "Load Game",
                Width = 100,
                Height = 30,
                Location = new Point(btnSave.Right + 10, btnSave.Top),
                BackColor = Color.Lavender,
                FlatStyle = FlatStyle.Flat,
                Name = "btnLoad"
            };
            btnLoad.Click += BtnLoad_Click;
            sudokuPanel.Controls.Add(btnLoad);



        }




        private void ShowSettingsPage()
        {
            // Hide other panels
            if (startPanel != null) startPanel.Visible = false;
            if (sudokuPanel != null) sudokuPanel.Visible = false;
            if (loginPanel != null) loginPanel.Visible = false;
            if (registerPanel != null) registerPanel.Visible = false;

            if (settingsPanel == null)
            {
                InitializeSettingsPage();
            }
            settingsPanel.Visible = true;
        }

        private void InitializeSettingsPage()
        {
            settingsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightBlue
            };
            this.Controls.Add(settingsPanel);

            // Title
            Label lblTitle = new Label
            {
                Text = "Account Settings",
                Font = new Font("Arial", 16, FontStyle.Bold),
                AutoSize = true,
                Left = (this.Width - 200) / 2,
                Top = 30
            };
            settingsPanel.Controls.Add(lblTitle);

            // New Username
            Label lblNewUsername = new Label
            {
                Text = "New Username:",
                Left = (this.Width - 300) / 2,
                Top = lblTitle.Bottom + 20,
                Width = 100
            };
            settingsPanel.Controls.Add(lblNewUsername);

            TextBox txtNewUsername = new TextBox
            {
                Width = 150,
                Left = lblNewUsername.Right + 10,
                Top = lblNewUsername.Top
            };
            txtNewUsername.Text = _username; // prefill with current username
            settingsPanel.Controls.Add(txtNewUsername);

            // New Password
            Label lblNewPassword = new Label
            {
                Text = "New Password:",
                Left = lblNewUsername.Left,
                Top = lblNewUsername.Bottom + 20,
                Width = 100
            };
            settingsPanel.Controls.Add(lblNewPassword);

            TextBox txtNewPassword = new TextBox
            {
                Width = 150,
                Left = lblNewPassword.Right + 10,
                Top = lblNewPassword.Top,
                PasswordChar = '*'
            };
            settingsPanel.Controls.Add(txtNewPassword);

            // Update Button
            Button btnUpdate = new Button
            {
                Text = "Update",
                Width = 100,
                BackColor = Color.Lavender,
                FlatStyle = FlatStyle.Flat,
                Left = lblNewPassword.Left,
                Top = lblNewPassword.Bottom + 20
            };

            btnUpdate.Click += (s, e) =>
            {
                string newUsername = txtNewUsername.Text;
                string newPassword = txtNewPassword.Text;

                // Update user only if username or password has changed
                if (_currentUser.Username != newUsername || !string.IsNullOrEmpty(newPassword))
                {
                    bool success = UserManager.UpdateUser(_currentUser, newUsername, newPassword);

                    if (success)
                    {
                        MessageBox.Show("Account updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Update the in-memory user safely
                        _currentUser.Username = newUsername;
                        if (!string.IsNullOrEmpty(newPassword))
                        {
                            _currentUser.Password = UserManager.HashPassword(newPassword); // Store hash
                        }

                        RefreshUI();
                        UpdateLeaderboard();
                    }
                    else
                    {
                        MessageBox.Show("Update failed. The new username might be taken.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };
            settingsPanel.Controls.Add(btnUpdate);

            // Delete Account Button
            Button btnDelete = new Button
            {
                Text = "Delete Account",
                Width = 120,
                Left = btnUpdate.Right + 20,
                Top = btnUpdate.Top,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LightCoral
            };
            btnDelete.Click += (s, e) =>
            {
                // Confirm deletion
                DialogResult dr = MessageBox.Show("Are you sure you want to delete your account? This action cannot be undone.",
                    "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    bool deleted = UserManager.DeleteUser(_currentUser.Username);  // Use _currentUser.Username
                    if (deleted)
                    {
                        MessageBox.Show("Account deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Reset _currentUser and _username after deletion
                        _currentUser = null;  // Set current user to null after deletion
                        _username = string.Empty; // Reset the _username variable as well

                        // Redirect to start page or log out the user
                        settingsPanel.Visible = false;
                        startPanel.Visible = true;
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete account.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            settingsPanel.Controls.Add(btnDelete);

            // Back button to return to the main game page
            Button btnBack = new Button
            {
                Text = "Back",
                Width = 100,
                BackColor = Color.Lavender,
                FlatStyle = FlatStyle.Flat,
                Left = btnUpdate.Left,
                Top = btnUpdate.Bottom + 20
            };
            btnBack.Click += (s, e) =>
            {
                settingsPanel.Visible = false;
                sudokuPanel.Visible = true; 
            };
            settingsPanel.Controls.Add(btnBack);
        }


        //private void LoginUser(string username, string password)
        //{
        //    User user = UserManager.GetUser(username, password);  // Fetch the user object
        //    //User user = UserManager.LoadUser(username);
        //    //User? user = LoadUser(username);  // Load the user from file
        //    //if (user != null && VerifyPassword(password, user.Password))

        //        if (user != null)
        //    {
        //        _currentUser = user;  // Store the user object in the current session
        //        _username = _currentUser.Username;  // Set _username with the logged-in user's username

        //        ShowSudokuPage();  // Show the main Sudoku page after successful login
        //        RefreshUI();  // Refresh the UI
        //    }
        //    else
        //    {
        //        MessageBox.Show("Invalid login. Please try again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        //private bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        //{
        //    string hashedEnteredPassword = HashPassword(enteredPassword);  // Hash the entered password
        //    return string.Equals(hashedEnteredPassword, storedHashedPassword, StringComparison.Ordinal);
        //}

        private void LoginUser(string username, string password)
        {
            // Load the user from the file based on the username
            User user = UserManager.LoadUser(username);

            if (user != null)
            {
                // Verify the entered password by comparing the hash of the entered password with the stored hash
                if (VerifyPassword(password, user.Password))
                {
                    _currentUser = user;  // Store the user object in the current session
                    _username = _currentUser.Username;  // Set _username with the logged-in user's username

                    ShowSudokuPage();  // Show the main Sudoku page after successful login
                    RefreshUI();  // Refresh the UI
                }
                else
                {
                    MessageBox.Show("Invalid login. Please try again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("User not found.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            // Hash the entered password
            string hashedEnteredPassword = UserManager.HashPassword(enteredPassword);

            // Compare the entered password hash with the stored hash
            return string.Equals(hashedEnteredPassword, storedHashedPassword, StringComparison.Ordinal);
        }


        private void CancelLogin()
        {
            if (loginPanel != null)
                loginPanel.Visible = false;
            if (startPanel != null)
                startPanel.Visible = true;
        }


        private void btnLogout_Click(object sender, EventArgs e)
        {
            // Log out: reset the current user
            _currentUser = null;

            // Reset playtime
            playTime = TimeSpan.Zero;
            lblPlayTime.Text = "Playtime: 00:00:00";

            ShowLoginPage();
            RefreshUI();
            sudokuGrid.ClearBoard();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetPuzzle();
        }

        private void btnHint_Click(object sender, EventArgs e)
        {
            if (puzzle == null)
            {
                MessageBox.Show("Please generate a puzzle first.");
                return;
            }

            if (sudokuGrid == null)
            {
                MessageBox.Show("Error: Sudoku grid is not initialized.");
                return;
            }

            if (lblHintCounter == null)
            {
                MessageBox.Show("Error: Hint counter label is missing.");
                return;
            }

            if (hintCount >= maxHints)
            {
                MessageBox.Show("Maximum hints used.");
                return;
            }

            int hintRow, hintCol, hintValue;
            if (SudokuSolver.GetHint(puzzle, out hintRow, out hintCol, out hintValue))
            {
                sudokuGrid.ShowHint(hintRow, hintCol, hintValue);
                hintCount++;
                lblHintCounter.Text = $"Hints: {hintCount}/{maxHints}";
            }
            else
            {
                MessageBox.Show("No hint available.");
            }
        }

        private void PlayTimer_Tick(object sender, EventArgs e)
        {
            playTime = playTime.Add(TimeSpan.FromSeconds(1));
            lblPlayTime.Text = "Playtime: " + playTime.ToString(@"hh\:mm\:ss");

            // If a user is logged in, update their total playtime.
            if (_currentUser != null)
            {
                _currentUser.Settings.TotalPlayTime = _currentUser.Settings.TotalPlayTime.Add(TimeSpan.FromSeconds(1));
                // save progress periodically:
                UserManager.SaveUser(_currentUser);
            }
        }

   


        private void UpdateLeaderboard()
        {
            lvLeaderboard.Items.Clear();

            string usersDir = Path.Combine("Data", "Users");
            if (Directory.Exists(usersDir))
            {
                // Get all user files (assuming .json files)
                var userFiles = Directory.GetFiles(usersDir, "*.json");
                var users = new List<User>();

                foreach (var file in userFiles)
                {
                    try
                    {
                        string json = File.ReadAllText(file);
                        var user = JsonSerializer.Deserialize<User>(json);
                        if (user != null)
                            users.Add(user);
                    }
                    catch (Exception ex)
                    {
                        // Log or handle deserialization errors as needed.
                        Logger.Log($"Error reading user file {file}: {ex.Message}", "ERROR");
                    }
                }

                // Sort users by FinishedPuzzleCount descending
                var sortedUsers = users.OrderByDescending(u => u.Settings.FinishedPuzzleCount).ToList();

                // Add sorted users to the ListView
                foreach (var user in sortedUsers)
                {
                    var item = new ListViewItem(user.Username);
                    item.SubItems.Add(user.Settings.FinishedPuzzleCount.ToString());
                    lvLeaderboard.Items.Add(item);
                }
            }
        }




        private async Task btnGenerate_Click(object sender, EventArgs e)
        {
            // Disable buttons and update UI elements
            Button btnGenerate = (Button)sudokuPanel.Controls["btnGenerate"];
            Button btnSolve = (Button)sudokuPanel.Controls["btnSolve"];
            Button btnReset = (Button)sudokuPanel.Controls["btnReset"];
            Button btnHint = (Button)sudokuPanel.Controls["btnHint"];
            btnGenerate.Enabled = false;
            btnSolve.Enabled = false;
            btnReset.Enabled = false;
            btnHint.Enabled = false;

            // Reset hints etc.
            hintCount = 0;
            if (lblHintCounter != null)
            {
                lblHintCounter.Text = $"Hints: {hintCount}/{maxHints}";
            }

            // Cancel any previous puzzle generation and create a new cancellation token
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;

            try
            {
                // Run puzzle generation in a separate thread
                puzzle = await Task.Run(() => SudokuGenerator.Generate(gridSize, token), token);

                // Update UI with the generated puzzle
                this.Invoke((MethodInvoker)delegate
                {
                    sudokuGrid.SetPuzzle(puzzle.Board);
                    Logger.Log($"New puzzle generated ({gridSize}x{gridSize})");
                });
            }
            catch (OperationCanceledException)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    Logger.Log("Puzzle generation was canceled.");
                });
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    Logger.Log($"Error generating puzzle: {ex.Message}");
                });
            }
            finally
            {
                // Re-enable buttons
                this.Invoke((MethodInvoker)delegate
                {
                    btnGenerate.Enabled = true;
                    btnSolve.Enabled = true;
                    btnReset.Enabled = true;
                    btnHint.Enabled = true;
                });

            }
        }


        private async Task SolvePuzzleAsync()
        {
            // Disable buttons during solving
            Button btnGenerate = (Button)sudokuPanel.Controls["btnGenerate"];
            Button btnSolve = (Button)sudokuPanel.Controls["btnSolve"];
            Button btnReset = (Button)sudokuPanel.Controls["btnReset"];
            Button btnHint = (Button)sudokuPanel.Controls["btnHint"];
            btnGenerate.Enabled = false;
            btnSolve.Enabled = false;
            btnReset.Enabled = false;
            btnHint.Enabled = false;

            if (puzzle == null)
            {
                MessageBox.Show("Generate a puzzle first.");
                btnGenerate.Enabled = true;
                btnSolve.Enabled = true;
                return;
            }

            // Record hint positions and user input (UI-specific logic)
            var hintPositions = new List<(int row, int col)>();
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    SudokuCell cell = sudokuGrid.GetCell(i, j);
                    if (cell.IsHint)
                        hintPositions.Add((i, j));
                }
            }

            int[,] userInputs = new int[gridSize, gridSize];
            bool[,] wasEditable = new bool[gridSize, gridSize];
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    SudokuCell cell = sudokuGrid.GetCell(i, j);
                    userInputs[i, j] = cell.Value;
                    wasEditable[i, j] = cell.IsEditable;
                }
            }

            // Solve the puzzle asynchronously
            bool solved = await SudokuSolver.SolvePuzzleAsync(puzzle);

            if (solved)
            {
                sudokuGrid.SetPuzzle(puzzle.Board);

                // Reapply UI-specific formatting
                foreach (var pos in hintPositions)
                {
                    SudokuCell hintCell = sudokuGrid.GetCell(pos.row, pos.col);
                    hintCell.Highlight(Color.LightYellow);
                    hintCell.IsHint = true;
                }

                for (int i = 0; i < gridSize; i++)
                {
                    for (int j = 0; j < gridSize; j++)
                    {
                        if (wasEditable[i, j])
                        {
                            SudokuCell cell = sudokuGrid.GetCell(i, j);
                            if (cell.IsHint)
                                continue;

                            int userVal = userInputs[i, j];
                            int correctVal = puzzle.Board[i, j];
                            if (userVal != 0)
                            {
                                cell.Highlight(userVal == correctVal ? Color.LightGreen : Color.LightCoral);
                            }
                        }
                    }
                }

                // Only update if a user is logged in
                if (_currentUser != null)
                {
                    // Increase finished puzzle count
                    _currentUser.Settings.FinishedPuzzleCount++;

                    // Save progress
                    UserManager.SaveUser(_currentUser);

                    MessageBox.Show($"Solved! Puzzles completed: {_currentUser.Settings.FinishedPuzzleCount}");
                    Logger.Log($"Puzzle solved by {_currentUser.Username}. Total completed: {_currentUser.Settings.FinishedPuzzleCount}", "INFO");

                    // Update leaderboard after successful completion
                    UpdateLeaderboard();
                }
                else
                {
                    // Handle the case for non-logged-in users if needed
                    MessageBox.Show("Solved!");
                }
            }
            else
            {
                MessageBox.Show("No solution found.");
                Logger.Log("Solve failed.", "ERROR");
            }

            btnGenerate.Enabled = true;
            btnSolve.Enabled = true;
            btnReset.Enabled = true;
            btnHint.Enabled = true;
        }

        private void ResetPuzzle()
        {
            Logger.Log("Resetting filled-in cells.");
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    SudokuCell cell = sudokuGrid.GetCell(i, j);
                    // Only reset editable (non-pre-filled) cells
                    if (cell.IsEditable && !cell.IsPreFilled)
                    {
                        cell.SetText("");
                        cell.Value = 0;
                        cell.BackColor = Color.White;
                    }
                    if (cell.IsHint)
                    {
                        cell.IsHint = false;
                        cell.BackColor = Color.White;
                    }
                }
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            // Adjust the start page button locations
            if (startPanel != null)
            {
                CenterControl(startPanel.Controls["startGameButton"]);
                CenterControl(startPanel.Controls["loginButton"]);
            }

            // Adjust the Sudoku page layout
            if (sudokuPanel != null)
            {
                AdjustSudokuLayout();
            }
        }

        private void CenterControl(Control control)
        {
            if (control != null)
            {
                control.Left = (this.Width - control.Width) / 2;
                control.Top = (this.Height - control.Height) / 2;
            }
        }

        private void AdjustSudokuLayout()
        {
            if (sudokuGrid != null)
            {
                sudokuGrid.Size = new Size(this.Width - 20, this.Height - 100);
            }

            if (btnReset != null && btnHint != null)
            {
                btnReset.Left = 10;
                btnReset.Top = sudokuGrid.Bottom + 10;

                btnHint.Left = btnReset.Right + 10;
                btnHint.Top = sudokuGrid.Bottom + 10;
            }

            if (sudokuPanel != null)
            {
                ComboBox cmbSize = (ComboBox)sudokuPanel.Controls["cmbSize"];
                if (cmbSize != null)
                {
                    cmbSize.Left = 10;
                    cmbSize.Top = btnHint.Bottom + 10;
                }

                Button btnGenerate = (Button)sudokuPanel.Controls["btnGenerate"];
                if (btnGenerate != null && cmbSize != null)
                {
                    btnGenerate.Left = cmbSize.Right + 10;
                    btnGenerate.Top = cmbSize.Top;
                }

                Button btnSolve = (Button)sudokuPanel.Controls["btnSolve"];
                if (btnSolve != null && btnGenerate != null)
                {
                    btnSolve.Left = btnGenerate.Right + 10;
                    btnSolve.Top = cmbSize.Top;
                }
            }
        }


        private void RefreshUI()
        {
            // Remove any existing login/logout-related controls first,
            // including lblPlayTime so that we don't keep a stale one.
            var existingControls = sudokuPanel.Controls.OfType<Control>()
                .Where(ctrl => ctrl.Name == "lblUser" ||
                               ctrl.Name == "btnGoToLogin" ||
                               ctrl.Name == "btnLogout" ||
                               ctrl.Name == "btnSettings" ||
                               ctrl.Name == "lblPlayTime")
                .ToList();

            foreach (var control in existingControls)
            {
                sudokuPanel.Controls.Remove(control);
            }

            // Add controls based on the current login state
            if (_currentUser == null)
            {
                // User is not logged in, show the "Go to Login" button
                Label lblUser = new Label
                {
                    Text = "Not logged in",
                    AutoSize = true,
                    BackColor = Color.HotPink,
                    Left = sudokuGrid.Right + 10,
                    Top = sudokuGrid.Top,
                    Name = "lblUser"
                };
                sudokuPanel.Controls.Add(lblUser);

                Button btnGoToLogin = new Button
                {
                    Text = "Go to Login",
                    Width = 150,
                    Height = 50,
                    BackColor = Color.Lavender,
                    FlatStyle = FlatStyle.Flat,
                    Left = lblUser.Left,
                    Top = lblUser.Bottom + 10,
                    Name = "btnGoToLogin"
                };
                btnGoToLogin.Click += BtnGoToLogin_Click;
                sudokuPanel.Controls.Add(btnGoToLogin);
            }
            else
            {
                // User is logged in, show a label with the username
                Label lblUser = new Label
                {
                    Text = $"Logged in as {_currentUser.Username}",
                    AutoSize = true,
                    BackColor = Color.HotPink,
                    Left = sudokuGrid.Right + 10,
                    Top = sudokuGrid.Top,
                    Name = "lblUser"
                };
                sudokuPanel.Controls.Add(lblUser);

                // Create and add the playtime label
                lblPlayTime = new Label
                {
                    Name = "lblPlayTime",
                    Text = "Playtime: 00:00:00",
                    AutoSize = true,
                    Location = new Point(lblUser.Left, lblUser.Bottom + 10)
                };
                sudokuPanel.Controls.Add(lblPlayTime);

                // Initialize and start the timer (if not already done)
                if (playTimer == null)
                {
                    playTimer = new System.Windows.Forms.Timer();
                    playTimer.Interval = 1000; // 1 second
                    playTimer.Tick += PlayTimer_Tick;
                    playTimer.Start();
                }


                // Update the playtime label
                playTime = _currentUser.Settings.TotalPlayTime;
                lblPlayTime.Text = "Playtime: " + playTime.ToString(@"hh\:mm\:ss");

                Button btnSettings = new Button
                {
                    Text = "Settings",
                    Width = 100,
                    Height = 40,
                    BackColor = Color.Lavender,
                    FlatStyle = FlatStyle.Flat,
                    Left = lblPlayTime.Left,
                    Top = lblPlayTime.Bottom + 10,
                    Name = "btnSettings"
                };
                btnSettings.Click += (s, e) => { ShowSettingsPage(); };
                sudokuPanel.Controls.Add(btnSettings);

                Button btnLogout = new Button
                {
                    Text = "Log Out",
                    Width = 100,
                    Height = 40,
                    BackColor = Color.LightCoral,
                    FlatStyle = FlatStyle.Flat,
                    Left = lblUser.Left,
                    Top = btnSettings.Bottom + 10,
                    Name = "btnLogout"
                };
                btnLogout.Click += btnLogout_Click;
                sudokuPanel.Controls.Add(btnLogout);
            }
            UpdateLeaderboard();
        }


        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Ensure that the user is logged in
            if (_currentUser == null)
            {
                MessageBox.Show("Please log in to save your game.", "Not Logged In", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the current board state from the sudokuGrid control
            CellState[,] currentBoard = sudokuGrid.GetBoardState();

            // Get the current grid size (this could be stored in a variable like `gridSize`)
            int currentGridSize = gridSize; // Or dynamically set this based on the selected size

            // Save the current game state using the username and grid size
            SudokuManager.SaveGame(_currentUser.Username, currentBoard, currentGridSize);

            MessageBox.Show("Game saved successfully!", "Save Game", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void BtnLoad_Click(object sender, EventArgs e)
        {
            // Ensure that the user is logged in
            if (_currentUser == null)
            {
                MessageBox.Show("Please log in to load your game.", "Not Logged In", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Load the saved game board for the current user
                var (loadedBoard, loadedGridSize) = SudokuManager.LoadGame(_currentUser.Username);

                // Update the ComboBox to reflect the loaded grid size
                cmbSize.SelectedItem = loadedGridSize == 4 ? "4x4" : loadedGridSize == 9 ? "9x9" : "16x16";

                // Update the Sudoku grid with the loaded board
                sudokuGrid.LoadBoardState(loadedBoard);

                // Make sure to update the puzzle object for the hint generator
                puzzle = new SudokuPuzzle(loadedGridSize);  // Use the loaded grid size

                // Copy the values from the loaded board to the puzzle's Board
                for (int i = 0; i < loadedBoard.GetLength(0); i++)
                {
                    for (int j = 0; j < loadedBoard.GetLength(1); j++)
                    {
                        puzzle.Board[i, j] = loadedBoard[i, j].Value;  // Copy each value to the puzzle's Board
                    }
                }

                // Optionally update any additional UI elements
                MessageBox.Show("Game loaded successfully!", "Load Game", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (FileNotFoundException ex)
            {
                // Show an error popup if no saved game is found
                MessageBox.Show("No saved game found for this user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Handle any other exceptions that might occur
                MessageBox.Show($"An error occurred while loading the game: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }






    }
}
