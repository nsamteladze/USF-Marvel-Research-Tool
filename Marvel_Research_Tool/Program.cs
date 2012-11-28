using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Collections;
using Marvel_Research_Tool.Utils;
using Marvel_Research_Tool.Command_Line;

namespace Marvel_Research_Tool
{
    public class Program
    {
        public static string DATA_DIR;
        public static string INPUT;
        public static string COMMAND_LINE;

        public const string SETTINGS = "settings.config";

        private const string KEY_DATA_DIR = "main_folder";
        private const string KEY_COMMAND_LINE = "command_line";
        private const string KEY_INPUT = "input";

        public static void Main(string[] args)
        {
            if (!LoadSettings())
            {
                FailAndExit("Cannot load application's settings.");
            }

            ApplicationCommandLine commandLine = new ApplicationCommandLine(COMMAND_LINE);
            commandLine.Start();

            Console.WriteLine("Press any key to exit the application . . .");
            Console.ReadKey();
        }

        private static bool LoadSettings()
        {
            if (!File.Exists(SETTINGS)) return false;

            Dictionary<string, string> settings = new Dictionary<string, string>();

            using (StreamReader reader = new StreamReader(SETTINGS))
            {
                while (!reader.EndOfStream)
                {
                    string input = reader.ReadLine();
                    if (input.Equals(String.Empty)) continue;

                    string[] inputParts = input.Split(' ');
                    if (inputParts.Length != 2) return false;
                    settings.Add(inputParts[0], inputParts[1]);
                }
            }

            if ((!settings.TryGetValue(KEY_DATA_DIR, out DATA_DIR)) ||
                (!settings.TryGetValue(KEY_COMMAND_LINE, out COMMAND_LINE)) ||
                (!settings.TryGetValue(KEY_INPUT, out INPUT)))
                return false;

            return true;
        }

        private static void FailAndExit(string message)
        {
            Console.WriteLine(String.Format("FATAL ERROR! Marvel Research Tool failed.\nMessage: {0}\nPress any key to close the application . . .", message));
            Console.ReadKey();
            Environment.Exit(-1);
        }
    }
}
