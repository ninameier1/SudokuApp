using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using SudokuApp.Models;
using SudokuApp.Services;
using SudokuApp.Utils;
using SudokuApp.CustomControls;

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
                BackColor = Color.Lavender,
                Left = (this.Width - 150) / 2,
                Top = (this.Height - 50) / 2,
                Name = "startGameButton"
            };
            startGameButton.Click += StartGameButton_Click;
            startPanel.Controls.Add(startGameButton);

            // Add a login button
            Button loginButton = new Button
            {
                Text = "Login/Register",
                Width = 150,
                Height = 50,
                BackColor = Color.Lavender,
                Left = (this.Width - 150) / 2,
                Top = startGameButton.Bottom + 10,
                Name = "loginButton"
            };
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





        private void StartGameButton_Click(object sender, EventArgs e)
        {
            ShowSudokuPage();
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

            RefreshUI();
        }

        private void BtnGoToLogin_Click(object sender, EventArgs e)
        {
            // Hide Sudoku page
            if (sudokuPanel != null)
                sudokuPanel.Visible = false;

            // Show Login page
            ShowLoginPage();
        }


        //private void BtnGoToLogin_Click(object sender, EventArgs e)
        //{
        //    // Show the login page
        //    ShowLoginPage();
        //}

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

            // ComboBox for puzzle size
            ComboBox cmbSize = new ComboBox
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
                Name = "btnGenerate"
            };
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
                Name = "btnReset"
            };
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
                Name = "btnHint"
            };
            btnHint.Click += btnHint_Click;
            sudokuPanel.Controls.Add(btnHint);

            // Add hint counter
            Label lblHintCounter = new Label
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
                Name = "btnSolve"
            };
            btnSolve.Click += async (s, e) => await SolvePuzzleAsync();
            sudokuPanel.Controls.Add(btnSolve);

            //if (string.IsNullOrEmpty(_username))
            //{
            //    // User is not logged in, show a label telling them they aren't logged in
            //    Label lblUser = new Label
            //    {
            //        Text = $"Not logged in",
            //        AutoSize = true,
            //        BackColor = Color.HotPink,
            //        Left = sudokuGrid.Right + 10,
            //        Top = sudokuGrid.Top + 200,
            //        Name = "lblUser"
            //    };
            //    sudokuPanel.Controls.Add(lblUser);
            //    // User is not logged in, show the "Go Back" button
            //    Button btnGoToLogin = new Button
            //    {
            //        Text = "Go to Login",
            //        Width = 150,
            //        Height = 50,
            //        BackColor = Color.Lavender,
            //        Left = sudokuGrid.Right + 10,
            //        Top = sudokuGrid.Top,
            //        Name = "btnGoToLogin"
            //    };
            //    btnGoToLogin.Click += BtnGoToLogin_Click;
            //    sudokuPanel.Controls.Add(btnGoToLogin);

            //}
            //else
            //{
            //    // User is logged in, show a label with the username
            //    Label lblUser = new Label
            //    {
            //        Text = $"Logged in as {_username}",
            //        AutoSize = true,
            //        BackColor = Color.HotPink,
            //        Left = sudokuGrid.Right + 10,
            //        Top = sudokuGrid.Top,
            //        Name = "lblUser"
            //    };
            //    sudokuPanel.Controls.Add(lblUser);

            //    // Add a Settings button below the username label
            //    Button btnSettings = new Button
            //    {
            //        Text = "Settings",
            //        Width = 100,
            //        Height = 40,
            //        BackColor = Color.LightSteelBlue,
            //        Left = lblUser.Left,
            //        Top = lblUser.Bottom + 10,
            //        Name = "btnSettings"
            //    };
            //    btnSettings.Click += (s, e) => { ShowSettingsPage(); };
            //    sudokuPanel.Controls.Add(btnSettings);

            //    // Log Out button
            //    Button btnLogout = new Button
            //    {
            //        Text = "Log Out",
            //        Width = 100,
            //        Height = 40,
            //        BackColor = Color.LightCoral,
            //        Left = lblUser.Left,
            //        Top = btnSettings.Bottom + 10,
            //        Name = "btnLogout"
            //    };
            //    btnLogout.Click += btnLogout_Click;
            //    sudokuPanel.Controls.Add(btnLogout);
            //}
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
                Left = lblNewPassword.Left,
                Top = lblNewPassword.Bottom + 20
            };
            btnUpdate.Click += (s, e) =>
            {
                // Call UpdateUser
                bool success = UserManager.UpdateUser(_username, txtNewUsername.Text, txtNewPassword.Text);
                if (success)
                {
                    MessageBox.Show("Account updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _username = txtNewUsername.Text; // update the global username variable
                    RefreshUI();
                }
                else
                {
                    MessageBox.Show("Update failed. The new username might be taken.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                BackColor = Color.LightCoral
            };
            btnDelete.Click += (s, e) =>
            {
                // Confirm deletion
                DialogResult dr = MessageBox.Show("Are you sure you want to delete your account? This action cannot be undone.",
                    "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    bool deleted = UserManager.DeleteUser(_username);
                    if (deleted)
                    {
                        MessageBox.Show("Account deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _username = string.Empty;
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


        private void LoginUser(string username, string password)
        {
            if (UserManager.Login(username, password))
            {
                _username = username;
                ShowSudokuPage();
                RefreshUI();
            }
            else
            {
                MessageBox.Show("Invalid login. Please try again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelLogin()
        {
            if (loginPanel != null)
                loginPanel.Visible = false;
            if (startPanel != null)
                startPanel.Visible = true;
        }

        //private void btnLogout_Click(object sender, EventArgs e)
        //{
        //    // Log out: reset the username
        //    _username = string.Empty;

        //    // Find the label and update its text
        //    Label lblUser = (Label)sudokuPanel.Controls["lblUser"];
        //    if (lblUser != null)
        //    {
        //        lblUser.Text = "Not Logged In"; // Or any other appropriate message
        //    }

        //    // Optionally, hide the sudoku panel and show the start panel
        //    sudokuPanel.Visible = false;
        //    startPanel.Visible = true;
        //}



        private void btnLogout_Click(object sender, EventArgs e)
        {
            // Log out: reset the username
            _username = string.Empty;

            // Refresh the UI after logging out
            RefreshUI();

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

            // Check if the user has exceeded the hint limit
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

                MessageBox.Show("Solved!");
                Logger.Log("Puzzle successfully solved.", "INFO");
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
            // Remove any existing login/logout-related controls first
            var existingControls = sudokuPanel.Controls.OfType<Control>()
                .Where(ctrl => ctrl.Name == "lblUser" || 
                 ctrl.Name == "btnGoToLogin" ||
                 ctrl.Name == "btnLogout" ||
                 ctrl.Name == "btnSettings")
                .ToList();

            foreach (var control in existingControls)
            {
                sudokuPanel.Controls.Remove(control);
            }

            // Add controls based on the current login state
            if (string.IsNullOrEmpty(_username))
            {
                // User is not logged in, show the "Go to Login" button
                Label lblUser = new Label
                {
                    Text = $"Not logged in",
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
                    Text = $"Logged in as {_username}",
                    AutoSize = true,
                    BackColor = Color.HotPink,
                    Left = sudokuGrid.Right + 10,
                    Top = sudokuGrid.Top,
                    Name = "lblUser"
                };
                sudokuPanel.Controls.Add(lblUser);

                Button btnSettings = new Button
                {
                    Text = "Settings",
                    Width = 100,
                    Height = 40,
                    BackColor = Color.LightSteelBlue,
                    Left = lblUser.Left,
                    Top = lblUser.Bottom + 10,
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
                    Left = lblUser.Left,
                    Top = btnSettings.Bottom + 10,
                    Name = "btnLogout"
                };
                btnLogout.Click += btnLogout_Click;
                sudokuPanel.Controls.Add(btnLogout);
            }
        }







    }
}
