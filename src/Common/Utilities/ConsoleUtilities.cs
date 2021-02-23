using System;
using System.IO;
using System.Runtime.CompilerServices;
using Figgle;
using Microsoft.DotNet.PlatformAbstractions;

namespace Ollio.Utilities
{
    public class ConsoleUtilities
    {
        public static void PrintDebugMessage(string message, string prefix = null, [CallerMemberName] string caller = null)
        {
#if DEBUG
            PrintStatusMessage(message, "Debug", prefix, caller, ConsoleColor.Magenta, ConsoleColor.DarkGray);
#endif
        }

        public static void PrintErrorMessage(Exception exception, [CallerMemberName] string caller = null)
        {

#if DEBUG
            string message = $"{exception.ToString()}";
#else
            string message = $@"{exception.Message}";
#endif
            PrintStatusMessage(message, "Oops", null, caller, ConsoleColor.Red, ConsoleColor.White);
        }

        public static void PrintInfoMessage(string message, string prefix = null, [CallerMemberName] string caller = null)
        {
            PrintStatusMessage(message, "Info", prefix, caller, ConsoleColor.Blue);
        }

        public static void PrintSuccessMessage(string message, string prefix = null, [CallerMemberName] string caller = null)
        {
            PrintStatusMessage(message, "Success", prefix, caller, ConsoleColor.Green);
        }

        public static void PrintWarningMessage(string message, string prefix = null, [CallerMemberName] string caller = null)
        {
            PrintStatusMessage(message, "Warning", prefix, caller, ConsoleColor.Yellow);
        }

        public static void PrintStartupMessage()
        {
            string logo = FiggleFonts.Standard.Render("Ollio").TrimEnd();
            string[] logoSplit = logo.Split('\n');

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(logo);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string('=', (logoSplit[logoSplit.Length - 2].Length + 1)));
            ResetForegroundColor();
        }

        static string RenderEmoji(string emoji)
        {
            switch (RuntimeEnvironment.OperatingSystemPlatform)
            {
                case Platform.Windows:
                    return $"{emoji}";
                default:
                    if (emoji.Length == 1)
                    {
                        return $"{emoji}";
                    }
                    else
                    {
                        return $"{emoji} ";
                    }
            }
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

                ResetForegroundColor();
            }
        }

        static void ResetForegroundColor()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}