using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Ollio.Common
{
    public class Write
    {
        public static void Debug(string message, string prefix = null, [CallerMemberName] string caller = null)
        {
#if DEBUG
            PrintStatusMessage(message, "Debug", prefix, caller, ConsoleColor.Magenta, ConsoleColor.DarkGray);
#endif
        }

        public static void Error(Exception exception, [CallerMemberName] string caller = null)
        {

#if DEBUG
            string message = $"{exception.ToString()}";
#else
            string message = $@"{exception.Message}";
#endif
            PrintStatusMessage(message, "Oops", null, caller, ConsoleColor.Red, ConsoleColor.White);
        }

        public static void Info(string message, string prefix = null, [CallerMemberName] string caller = null)
        {
            PrintStatusMessage(message, "Info", prefix, caller, ConsoleColor.Blue);
        }

        public static void Success(string message, string prefix = null, [CallerMemberName] string caller = null)
        {
            PrintStatusMessage(message, "Success", prefix, caller, ConsoleColor.Green);
        }

        public static void Warning(string message, string prefix = null, [CallerMemberName] string caller = null)
        {
            PrintStatusMessage(message, "Warning", prefix, caller, ConsoleColor.Yellow);
        }

        public static void Reset()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void PrintStatusMessage(
            string message,
            string status = "",
            string prefix = "",
            string caller = "",
            ConsoleColor prefixColor = ConsoleColor.White,
            ConsoleColor messageColor = ConsoleColor.Gray
        )
        {
#if DEBUG
            if (!String.IsNullOrEmpty(caller))
                status = caller;
#endif

            if (!String.IsNullOrEmpty(message))
            {
                status = $" [{status}] ";
                int prefixLength = status.Length;
                string prefixSpacer = new string(' ', prefixLength);

                Console.ForegroundColor = prefixColor;
                Console.Write(status);

                if (!String.IsNullOrEmpty(prefix))
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write($"({prefix}) ");
                }

                Console.ForegroundColor = messageColor;

                using (var reader = new StringReader(message))
                {
                    bool firstLine = true;
                    for (
                        string messageLine = reader.ReadLine();
                        messageLine != null;
                        messageLine = reader.ReadLine()
                    )
                    {
                        if (!firstLine)
                            Console.Write(prefixSpacer);

                        Console.WriteLine(messageLine.Trim());
                        firstLine = false;
                    }
                }

                Reset();
            }
        }
    }
}