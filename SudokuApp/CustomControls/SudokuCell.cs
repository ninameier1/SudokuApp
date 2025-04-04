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

            // Set the font size to 65% of the cell size otherwise there's like a small white line for some reason
            float fontSize = cellSize * 0.65f;
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
                    Value = 0;
                }
                else
                {
                    Value = number;
                }
            }
            else if (!string.IsNullOrEmpty(textBox.Text))
            {
                MessageBox.Show("Only numbers are allowed.");
                textBox.Text = "";
                Value = 0;
            }
            else
            {
                Value = 0;
            }
        }

        public void Highlight(Color color)
        {
            this.BackColor = color;
            // Also update the textbox background so it isn’t overwritten by ApplyPreFilledStyle
            foreach (Control ctrl in this.Controls)
            {
                ctrl.BackColor = color;
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
            // Check if the cell is pre-filled
            if (IsPreFilled)
            {
                // If pre-filled, set the background to LightGray and set it to ReadOnly
                textBox.ReadOnly = true;
                if (textBox.BackColor != Color.LightGray) // Only change if it's not already LightGray
                {
                    textBox.BackColor = Color.LightGray;
                }
                textBox.ForeColor = Color.Black;
            }
            else
            {
                // If not pre-filled, set it based on whether it's editable or not
                textBox.ReadOnly = !IsEditable;

                // Set background based on editability
                textBox.BackColor = IsEditable ? Color.White : Color.LightGray;

                textBox.ForeColor = Color.Black;
            }
        }

    }
}
