using System;
using System.IO;
using SimpleJSON;
using UTanksServer;
using UTanksServer.Database;

namespace Core
{
    public class Logger
    {
        public static JSONNode Config { get => Program.Config["Logger"]; }
        public static bool printDebugStuff { get => Config["PrintDebugVerbose"].AsBool; }
        public static void Log(string message = "", string name = "main")
        {
            string _text = $"{Format(name, "Info")}{message}";
            ConsoleColor PrevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            ClearCurrentConsoleLine();
            Console.WriteLine(_text);
            Console.ForegroundColor = PrevColor;
            RedrawInput();
            SaveToLogFile(_text);
        }

        public static void LogWarn(string message = "", string name = "main")
        {
            string _text = $"{Format(name, "Warn")}{message}";
            ConsoleColor PrevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            ClearCurrentConsoleLine();
            Console.WriteLine(_text);
            Console.ForegroundColor = PrevColor;
            RedrawInput();
            SaveToLogFile(_text);
        }

        public static void LogError(string message = "", string name = "main")
        {
            string _text = $"{Format(name, "Error")}{message}";
            ConsoleColor PrevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            ClearCurrentConsoleLine();
            Console.WriteLine(_text);
            Console.ForegroundColor = PrevColor;
            RedrawInput();
            SaveToLogFile(_text);
        }


        public static void LogDebug(string message = "", string name = "main")
        {
            string _text = $"{Format(name, "Debug")}{message}";
            if (printDebugStuff)
            {
                ConsoleColor PrevColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                ClearCurrentConsoleLine();
                Console.WriteLine(_text);
                Console.ForegroundColor = PrevColor;
            }
            RedrawInput();
            SaveToLogFile(_text);
        }

        public static void LogNetwork(string message = "", string name = "main")
        {
            string _text = $"{Format(name, "Net")}{message}";
            ConsoleColor PrevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            ClearCurrentConsoleLine();
            Console.WriteLine(_text);
            Console.ForegroundColor = PrevColor;
            RedrawInput();
            SaveToLogFile(_text);
        }

        static string ConsoleLabel = string.Empty;
        static string ConsoleInput = string.Empty;
        static string ConsolePassInput = string.Empty;
        public static string ReadLine(string label = "")
        {
            ConsoleLabel = label;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && ConsoleInput.Length > 0)
                {
                    if (ConsoleInput.Length == 1)
                        ConsoleInput = string.Empty;
                    else
                        ConsoleInput = ConsoleInput.Substring(0, ConsoleInput.Length - 1);
                    RedrawInput();
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    ConsoleInput += keyInfo.KeyChar;
                    RedrawInput();
                }
            } while (key != ConsoleKey.Enter);
            Console.WriteLine();
            string pass = ConsoleInput;
            ConsoleInput = string.Empty;
            ConsoleLabel = string.Empty;
            return pass;
        }

        static string Format(string subject, string type)
        {
            string additionalSpaceAfterType = new string(' ', 5 - type.Length);
            string format = $"[{DateTime.Now.ToString((string)Config["DateTimeFormat"])}] [{type}]{additionalSpaceAfterType} [{subject}]";
            format += $"{new string(' ', Config["CaretStartPos"].AsInt - format.Length < 0 ? 0 : Config["CaretStartPos"].AsInt - format.Length)}: ";
            return format;
        }

        public static string ReadPass(string label = "")
        {
            ConsoleLabel = label;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && ConsoleInput.Length > 0)
                {
                    if (ConsoleInput.Length == 1)
                    {
                        ConsoleInput = string.Empty;
                        ConsolePassInput = string.Empty;
                    }
                    else
                    {
                        ConsoleInput = ConsoleInput.Substring(0, ConsoleInput.Length - 1);
                        ConsolePassInput = string.Empty;
                    }
                    RedrawInput();
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    ConsoleInput += '*';
                    ConsolePassInput += keyInfo.KeyChar;
                    RedrawInput();
                }
            } while (key != ConsoleKey.Enter);
            Console.WriteLine();
            string pass = ConsolePassInput;
            ConsoleInput = string.Empty;
            ConsolePassInput = string.Empty;
            ConsoleLabel = string.Empty;
            return pass;
        }

        public static void RedrawInput()//hidden crusher in linux
        {
            //ClearCurrentConsoleLine();
            //Console.Write(ConsoleLabel + ConsoleInput);
        }

        public static void ClearCurrentConsoleLine()//hidden crusher in linux
        {
            //int currentLineCursor = Console.CursorTop;
            //Console.SetCursorPosition(0, Console.CursorTop);
            //Console.Write(new string(' ', Console.WindowWidth));
            //Console.SetCursorPosition(0, currentLineCursor);
        }

        static void SaveToLogFile(string text)
        {
            if (!Config["ExportLogsToFile"]) return;
            return;
            string path = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(path))
                 Directory.CreateDirectory(path);

             string _filePath = Path.Join(path, DateTime.Now.ToString("yyyy-M-dd") + ".log");
             if (File.Exists(_filePath))
                using (StreamWriter _sw = File.AppendText(_filePath))
                    _sw.Write("\n" + text);
             else File.WriteAllText(_filePath, text);
        }
    }
}