using System;

namespace UTanksServer.Core.Logging
{
    internal static class Logger
    {
        private static readonly object _lock = new();

        private static void Write(string type, ConsoleColor color, object content)
        {
            lock (_lock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine($"[{DateTime.UtcNow}, {type}] {content}");
            }
        }

        private static void DebugWrite(string type, ConsoleColor color, object content)
        {
#if DEBUG
            Write(type, color, content);
#endif
        }

        public static void Log(object content) => Write("INFO", ConsoleColor.Gray, content);

        public static void Debug(object content) => DebugWrite("DEBUG", ConsoleColor.DarkGreen, content);
        public static void Trace(object content)
        {
            //if (Server.Instance.Settings.EnableTracing)
            //    DebugWrite("TRACE", ConsoleColor.DarkGray, content);
        }

        public static void Warn(object content) => Write("WARN", ConsoleColor.DarkYellow, content);

        public static void Error(object content) => Write("ERROR", ConsoleColor.Red, content);
    }
}
