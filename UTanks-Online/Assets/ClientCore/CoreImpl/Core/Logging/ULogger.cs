using SecuredSpace.ClientControl.Model;
using System;
using UnityEngine;

namespace UTanksClient.Core.Logging
{
    internal class ULogger : IService
    {
        public static ULogger instance => IService.getInstance<ULogger>();

        private static readonly object _lock = new object();

        private static void Write(string type, ConsoleColor color, object content)
        {
            lock (_lock)
            {
                Console.ForegroundColor = color;
                if(ConsoleColor.Red.Equals(color))
                    Debug.LogError($"[{DateTime.UtcNow}, {type}] {content}");
                else
                    Debug.Log($"[{DateTime.UtcNow}, {type}] {content}");
            }
        }

        private static void DebugWrite(string type, ConsoleColor color, object content)
        {
#if DEBUG
            Write(type, color, content);
#endif
        }

        public static void Log(object content) => Write("INFO", ConsoleColor.Gray, content);

        public static void LogLoad(object content) => Write("LoadingLog", ConsoleColor.Gray, content);

        public static void LogDebug(object content) => DebugWrite("DEBUG", ConsoleColor.DarkGreen, content);
        public static void Trace(object content)
        {
            //if (Server.Instance.Settings.EnableTracing)
            //    DebugWrite("TRACE", ConsoleColor.DarkGray, content);
        }

        public static void Warn(object content) => Write("WARN", ConsoleColor.DarkYellow, content);

        public static void Error(object content) => Write("ERROR", ConsoleColor.Red, content);

        public override void PostInitializeProcess()
        {
            
        }

        public override void InitializeProcess()
        {
            
        }

        public override void OnDestroyReaction()
        {
            
        }
    }
}
