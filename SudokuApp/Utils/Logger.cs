using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuApp.Utils
{
    public static class Logger
    {
        private static readonly string logFile = "app.log";

        public static void Log(string message, string level = "INFO")
        {
            string logMessage = $"{DateTime.Now} [{level}] {message}";
            File.AppendAllText(logFile, logMessage + Environment.NewLine);
        }
    }
}
