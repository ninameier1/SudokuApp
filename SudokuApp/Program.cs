using System;
using System.Windows.Forms;
using SudokuApp.Forms;

namespace SudokuApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(""));

        }
    }
}
