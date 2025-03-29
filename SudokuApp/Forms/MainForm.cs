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
        private SudokuGrid sudokuGrid;
        private Button resetButton;
        private Button hintButton;
        private int gridSize = 9;

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
                Left = (this.Width - 150) / 2,
                Top = (this.Height - 50) / 2,
                Name = "startGameButton"
            };
            startGameButton.Click += StartGameButton_Click;
            startPanel.Controls.Add(startGameButton);

            // Add a login button
            Button loginButton = new Button
            {
                Text = "Login",
                Width = 150,
                Height = 50,
                Left = (this.Width - 150) / 2,
                Top = startGameButton.Bottom + 10,
                Name = "loginButton"
            };
            loginButton.Click += LoginButton_Click;
            startPanel.Controls.Add(loginButton);
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

            // Initialize and show the Sudoku game page if not already created
            if (sudokuPanel == null)
            {
                InitializeSudokuPage();
            }
            sudokuPanel.Visible = true;
        }

        private void InitializeSudokuPage()
        {
            sudokuPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightGray
            };
            this.Controls.Add(sudokuPanel);

            // Initialize the SudokuGrid control and pass gridSize to it
            sudokuGrid = new SudokuGrid(gridSize)
            {
                Location = new Point(10, 10)
            };
            sudokuPanel.Controls.Add(sudokuGrid);

            // Add reset button
            resetButton = new Button
            {
                Text = "Reset",
                Location = new Point(10, sudokuGrid.Bottom + 10),
                Name = "resetButton"
            };
            resetButton.Click += ResetButton_Click;
            sudokuPanel.Controls.Add(resetButton);

            // Add hint button
            hintButton = new Button
            {
                Text = "Show Hint",
                Location = new Point(resetButton.Right + 10, sudokuGrid.Bottom + 10),
                Name = "hintButton"
            };
            hintButton.Click += HintButton_Click;
            sudokuPanel.Controls.Add(hintButton);

            // ComboBox for puzzle size
            ComboBox cmbSize = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Left = 10,
                Top = hintButton.Bottom + 10,
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
                Location = new Point(cmbSize.Right + 10, cmbSize.Top),
                Name = "btnGenerate"
            };
            btnGenerate.Click += (s, e) => GeneratePuzzle();
            sudokuPanel.Controls.Add(btnGenerate);

            // Button for solving the puzzle
            Button btnSolve = new Button
            {
                Text = "Solve",
                Location = new Point(btnGenerate.Right + 10, cmbSize.Top),
                Name = "btnSolve"
            };
            btnSolve.Click += async (s, e) => await SolvePuzzleAsync();
            sudokuPanel.Controls.Add(btnSolve);
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            ShowLoginPage();
        }

        private void ShowLoginPage()
        {
            // Hide the start page
            if (startPanel != null)
                startPanel.Visible = false;

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
                BackColor = Color.Beige
            };
            this.Controls.Add(loginPanel);

            // Add login controls
            Label usernameLabel = new Label
            {
                Text = "Username:",
                Left = 10,
                Top = 10
            };
            loginPanel.Controls.Add(usernameLabel);

            TextBox usernameTextbox = new TextBox
            {
                Left = 10,
                Top = 30
            };
            loginPanel.Controls.Add(usernameTextbox);

            Label passwordLabel = new Label
            {
                Text = "Password:",
                Left = 10,
                Top = 60
            };
            loginPanel.Controls.Add(passwordLabel);

            TextBox passwordTextbox = new TextBox
            {
                Left = 10,
                Top = 80,
                PasswordChar = '*'
            };
            loginPanel.Controls.Add(passwordTextbox);

            Button loginButton = new Button
            {
                Text = "Login",
                Left = 10,
                Top = 110
            };
            loginButton.Click += (s, e) => LoginUser(usernameTextbox.Text, passwordTextbox.Text);
            loginPanel.Controls.Add(loginButton);

            Button cancelButton = new Button
            {
                Text = "Cancel",
                Left = 90,
                Top = 110
            };
            cancelButton.Click += (s, e) => CancelLogin();
            loginPanel.Controls.Add(cancelButton);
        }

        private void LoginUser(string username, string password)
        {
            if (UserManager.Login(username, password))
            {
                _username = username;
                ShowSudokuPage();
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

        private void ResetButton_Click(object sender, EventArgs e)
        {
            ResetPuzzle();
        }

        private void HintButton_Click(object sender, EventArgs e)
        {
            // Example: Show a hint for cell (0,0) with the value 5
            sudokuGrid.ShowHint(0, 0, 5);
        }
        private CancellationTokenSource _cancellationTokenSource = null;

        private async void GeneratePuzzle()
        {
            // Disable buttons
            Button btnGenerate = (Button)sudokuPanel.Controls["btnGenerate"];
            Button btnSolve = (Button)sudokuPanel.Controls["btnSolve"];
            btnGenerate.Enabled = false;
            btnSolve.Enabled = false;

            // Cancel the previous task if one is running
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;

            try
            {
                // Task.run the puzzle generation without blocking the UI thread
                await Task.Run(() =>
                {
                    // Check for cancellation before starting the task
                    if (token.IsCancellationRequested)
                        return;

                    // Generate puzzle with cancellation token
                    puzzle = SudokuGenerator.Generate(gridSize, token);  // Pass the cancellation token
                }, token);

                // If the task completes, update the UI
                this.Invoke((MethodInvoker)delegate
                {
                    sudokuGrid.SetPuzzle(puzzle.Board);
                    Logger.Log($"New puzzle generated ({gridSize}x{gridSize})");
                });
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation gracefully if needed
                this.Invoke((MethodInvoker)delegate
                {
                    Logger.Log("Puzzle generation was canceled.");
                });
            }
            finally
            {
                // Re-enable buttons after the task is completed or canceled
                this.Invoke((MethodInvoker)delegate
                {
                    btnGenerate.Enabled = true;
                    btnSolve.Enabled = true;
                });
            }
        }




        private async Task SolvePuzzleAsync()
        {
            // Disable buttons
            Button btnGenerate = (Button)sudokuPanel.Controls["btnGenerate"];
            Button btnSolve = (Button)sudokuPanel.Controls["btnSolve"];
            btnGenerate.Enabled = false;
            btnSolve.Enabled = false;

            if (puzzle == null)
            {
                MessageBox.Show("Generate a puzzle first.");
                // Re-enable buttons if puzzle is not generated
                btnGenerate.Enabled = true;
                btnSolve.Enabled = true;
                return;
            }

            bool solved = await Task.Run(() => SudokuSolver.Solve(puzzle));
            if (solved)
            {
                sudokuGrid.SetPuzzle(puzzle.Board);
                MessageBox.Show("Solved!");
                Logger.Log("Puzzle successfully solved.", "INFO");
            }
            else
            {
                MessageBox.Show("No solution found.");
                Logger.Log("Solve failed.", "ERROR");
            }

            // Re-enable buttons after solving is complete
            btnGenerate.Enabled = true;
            btnSolve.Enabled = true;
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

            if (resetButton != null && hintButton != null)
            {
                resetButton.Left = 10;
                resetButton.Top = sudokuGrid.Bottom + 10;

                hintButton.Left = resetButton.Right + 10;
                hintButton.Top = sudokuGrid.Bottom + 10;
            }

            if (sudokuPanel != null)
            {
                ComboBox cmbSize = (ComboBox)sudokuPanel.Controls["cmbSize"];
                if (cmbSize != null)
                {
                    cmbSize.Left = 10;
                    cmbSize.Top = hintButton.Bottom + 10;
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
    }
}
