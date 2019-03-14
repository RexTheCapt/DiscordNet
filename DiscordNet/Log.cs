#region usings

using System;

#endregion

namespace DiscordNet
{
    internal static class Log
    {
        /// <summary>
        ///     Write info messages with timestamps as default.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="showTime"></param>
        public static void Info(string message, bool showTime = true)
        {
            Write(message, showTime, ConsoleColor.Yellow);
        }

        public static void Error(object message, bool showTime = true)
        {
            Write(message.ToString(), showTime, ConsoleColor.Red);
        }
        public static void Error(string message, bool showTime = true)
        {
            Write(message, showTime, ConsoleColor.Red);
        }

        /// <summary>
        ///     Write an message with custom color.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="showTime"></param>
        /// <param name="color"></param>
        public static void Write(string message, bool showTime = true, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;

            string time = DateTime.Now.ToLongTimeString();
            string indent = "";

            if (!showTime)
            {
                for (int i = 0; i < time.Length; i++) indent += " ";

                time = indent;
            }

            Console.WriteLine($"{time} {message}");

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void DmWrite(string message)
        {
            Write(message, color:ConsoleColor.DarkGray);
        }
    }
}