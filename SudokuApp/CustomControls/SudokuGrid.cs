using System;
using System.Drawing;
using System.Windows.Forms;
using SudokuApp.Models;
using SudokuApp.Services;

namespace SudokuApp.CustomControls
{
    public partial class SudokuGrid : UserControl
    {
        private SudokuCell[,] cells;
        private int gridSize;  // dynamic grid size
        private int currentCellSize; // current cell size used for layout and drawing

        public SudokuGrid(int gridSize)
        {
            this.gridSize = gridSize;
            // Use a default cell size (will be recalculated on resize/update)
            currentCellSize = 40;
            this.Size = new Size(gridSize * currentCellSize, gridSize * currentCellSize);
            this.DoubleBuffered = true; // reduce flicker
            this.BorderStyle = BorderStyle.None;
            CreateCells();
        }

        // Creates/recreates the cell controls for the current gridSize.
        private void CreateCells()
        {
            this.Controls.Clear();
            cells = new SudokuCell[gridSize, gridSize];

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    SudokuCell cell = new SudokuCell(currentCellSize)
                    {
                        Location = new Point(j * currentCellSize + 1, i * currentCellSize + 1),
                        Size = new Size(currentCellSize - 2, currentCellSize - 2),
                        IsEditable = true,
                        Value = 0
                    };
                    // Set the maximum allowed value for this cell (so validation works for grids >9)
                    cell.MaxValue = gridSize;
                    this.Controls.Add(cell);
                    cells[i, j] = cell;
                }
            }
        }

        // Reinitializes the grid with a new size.
        public void UpdateGridSize(int newSize)
        {
            if (newSize <= 0 || Math.Sqrt(newSize) % 1 != 0)
                throw new ArgumentException("Puzzle size must be a perfect square.");

            gridSize = newSize;
            // Recalc cell size based on the current control size
            currentCellSize = Math.Min(this.Width, this.Height) / gridSize;
            if (currentCellSize < 20)
                currentCellSize = 20; // enforce a minimum size

            CreateCells();
            // Update overall control size
            this.Size = new Size(currentCellSize * gridSize, currentCellSize * gridSize);
            this.Invalidate();
        }

        // Sets the puzzle values into the grid.
        public void SetPuzzle(int[,] puzzle)
        {
            if (puzzle.GetLength(0) != gridSize || puzzle.GetLength(1) != gridSize)
                throw new ArgumentException("Puzzle size does not match the current grid size.");

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    SudokuCell cell = cells[i, j];
                    int value = puzzle[i, j];

                    cell.Value = value;
                    if (value != 0)
                    {
                        cell.SetText(value.ToString());
                        cell.IsEditable = false;
                        cell.IsPreFilled = true;
                    }
                    else
                    {
                        cell.SetText("");
                        cell.IsEditable = true;
                        cell.IsPreFilled = false;
                    }

                    // Reset any previous hint or highlight flags
                    cell.IsHint = false;

                    // Apply default styling
                    cell.ApplyPreFilledStyle();
                    cell.BackColor = cell.IsPreFilled ? Color.LightGray : Color.White;

                    // Also update the textbox background if needed:
                    foreach (Control ctrl in cell.Controls)
                    {
                        ctrl.BackColor = cell.BackColor;
                    }
                }
            }
        }

        //public void SetPuzzle(int[,] puzzle)
        //{
        //    if (puzzle.GetLength(0) != gridSize || puzzle.GetLength(1) != gridSize)
        //        throw new ArgumentException("Puzzle size does not match the current grid size.");

        //    for (int i = 0; i < gridSize; i++)
        //    {
        //        for (int j = 0; j < gridSize; j++)
        //        {
        //            SudokuCell cell = cells[i, j];
        //            int value = puzzle[i, j];

        //            cell.Value = value;
        //            if (value != 0)
        //            {
        //                cell.SetText(value.ToString());
        //                cell.IsEditable = false;
        //                cell.IsPreFilled = true;
        //            }
        //            else
        //            {
        //                cell.SetText("");
        //                cell.IsEditable = true;
        //                cell.IsPreFilled = false;
        //            }
        //            cell.ApplyPreFilledStyle();
        //        }
        //    }
        //}

        // Returns the cell at the specified row and column.
        public SudokuCell GetCell(int row, int col)
        {
            if (row < 0 || row >= gridSize || col < 0 || col >= gridSize)
                throw new ArgumentOutOfRangeException("Invalid cell coordinates");
            return cells[row, col];
        }

        // Displays a hint in the specified cell.
        public void ShowHint(int row, int col, int value)
        {
            // Use gridSize instead of hard-coded 9.
            if (row < 0 || row >= gridSize || col < 0 || col >= gridSize)
                return;

            SudokuCell cell = cells[row, col];
            if (cell != null && cell.IsEditable)
            {
                cell.SetText(value.ToString());
                cell.IsHint = true;
                cell.Highlight(Color.LightYellow);
            }
        }



        // Override OnPaint to draw grid lines (with thicker lines between blocks).
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int blockSize = (int)Math.Sqrt(gridSize);

            using (Pen thinPen = new Pen(Color.Black, 1))
            using (Pen thickPen = new Pen(Color.Black, 3))
            {
                for (int i = 0; i <= gridSize; i++)
                {
                    Pen currentPen = (i % blockSize == 0) ? thickPen : thinPen;
                    // Vertical lines
                    e.Graphics.DrawLine(currentPen, i * currentCellSize, 0, i * currentCellSize, gridSize * currentCellSize);
                    // Horizontal lines
                    e.Graphics.DrawLine(currentPen, 0, i * currentCellSize, gridSize * currentCellSize, i * currentCellSize);
                }
            }
        }
    }
}
