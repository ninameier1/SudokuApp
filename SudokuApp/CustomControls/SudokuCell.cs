using System;
using System.Drawing;
using System.Windows.Forms;
using SudokuApp.Models;
using SudokuApp.Services;

namespace SudokuApp.CustomControls
{
    public partial class SudokuCell : UserControl
    {
        private TextBox textBox;
        public bool IsPreFilled { get; set; }
        public int Value { get; set; }
        public bool IsEditable { get; set; }
        public bool IsHint { get; set; }

        // Property to control the maximum allowed value (e.g., 9 for 9x9, 16 for 16x16).
        private int _maxValue = 9;
        public int MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                textBox.MaxLength = _maxValue >= 10 ? 2 : 1;
            }
        }

        private bool validateInput = true;

        public SudokuCell(int cellSize)
        {
            // Set the control size
            this.Size = new Size(cellSize, cellSize);

            textBox = new TextBox
            {
                Dock = DockStyle.Fill,
                TextAlign = HorizontalAlignment.Center,
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
            };

            Controls.Add(textBox);

            // Remove extra padding/margins
            textBox.Padding = new Padding(0);
            textBox.Margin = new Padding(0);
            this.Padding = new Padding(0);
            this.Margin = new Padding(0);

            // Set the font size to 60% of the cell size
            float fontSize = cellSize * 0.6f;
            textBox.Font = new Font("Arial", fontSize);

            textBox.TextChanged += (sender, e) => OnTextChanged();

            textBox.KeyPress += (sender, e) =>
            {
                // Allow digits and control keys.
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            };
        }

        public void SetText(string text)
        {
            validateInput = false;
            textBox.Text = text;
            validateInput = true;
        }

        private void OnTextChanged()
        {
            if (!validateInput) return;

            if (int.TryParse(textBox.Text, out int number))
            {
                if (number < 1 || number > MaxValue)
                {
                    MessageBox.Show($"Please enter a number between 1 and {MaxValue}.");
                    textBox.Text = "";
                }
            }
            else if (!string.IsNullOrEmpty(textBox.Text))
            {
                MessageBox.Show("Only numbers are allowed.");
                textBox.Text = "";
            }
        }

        public void SetValueWithoutValidation(int number)
        {
            validateInput = false;
            textBox.Text = number.ToString();
            validateInput = true;
        }

        public void ApplyPreFilledStyle()
        {
            if (IsPreFilled)
            {
                textBox.ReadOnly = true;
                textBox.BackColor = Color.LightGray;
                textBox.ForeColor = Color.Black;
            }
            else
            {
                textBox.ReadOnly = !IsEditable;
                textBox.BackColor = IsEditable ? Color.White : Color.LightGray;
                textBox.ForeColor = Color.Black;
            }
        }
    }
}
