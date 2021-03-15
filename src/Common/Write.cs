using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Ollio.Common
{
    public class Write
    {
        public static void Debug(string message, [CallerMemberName] string caller = null)
        {
#if DEBUG
            Print(message, "Debug", caller, ConsoleColor.Magenta, ConsoleColor.DarkGray);
#endif
        }

        public static void Error(Exception exception, [CallerMemberName] string caller = null)
        {

#if DEBUG
            string message = $"{exception.ToString()}";
#else
            string message = $@"{exception.Message}";
#endif
            Print(message, "Oops", caller, ConsoleColor.Red, ConsoleColor.White);
        }

        public static void Info(string message, [CallerMemberName] string caller = null)
        {
            Print(message, "Info", caller, ConsoleColor.Blue);
        }

        public static void Success(string message, [CallerMemberName] string caller = null)
        {
            Print(message, "Success", caller, ConsoleColor.Green);
        }

        public static void Warning(string message, [CallerMemberName] string caller = null)
        {
            Print(message, "Warning", caller, ConsoleColor.Yellow);
        }

        public static void Reset()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void Print(
            string message,
            string status = "",
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