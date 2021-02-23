using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Figgle;
using Microsoft.DotNet.PlatformAbstractions;

namespace Mishmash.Utilities
{
    public class ConsoleUtilities
    {
        public static void PrintDebugMessage(string message, [CallerMemberName] string caller = null)
        {
#if DEBUG
            PrintStatusMessage(message, "Debug", caller, ConsoleColor.Magenta, ConsoleColor.DarkGray);
#endif
        }

        public static void PrintErrorMessage(Exception exception, [CallerMemberName] string caller = null)
        {
            
#if DEBUG
            string message = $"{exception.ToString()}";
#else
            string message = $@"{exception.Message}";
#endif
            PrintStatusMessage(message, "Oops", caller, ConsoleColor.Red, ConsoleColor.White);
        }

        public static void PrintSuccessMessage(string message, [CallerMemberName] string caller = null)
        {
            PrintStatusMessage(message, "Success", caller, ConsoleColor.Green);
        }

        public static void PrintWarningMessage(string message, [CallerMemberName] string caller = null)
        {
            PrintStatusMessage(message, "Warning", caller, ConsoleColor.Yellow, ConsoleColor.DarkGray);
        }

        public static void PrintStartupMessage()
        {
            //string logo = @" __  __ _     _     __  __           _     
//|  \/  (_)___| |__ |  \/  | __ _ ___| |__  
//| |\/| | / __| '_ \| |\/| |/ _` / __| '_ \ 
//| |  | | \__ \ | | | |  | | (_| \__ \ | | |
//|_|  |_|_|___/_| |_|_|  |_|\__,_|___/_| |_|
//===========================================";

            string logo = FiggleFonts.Standard.Render("MishMash").TrimEnd();
            string[] logoSplit = logo.Split('\n');

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(logo);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string('=', (logoSplit[logoSplit.Length-2].Length+1)));
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
            string prefix = "",
            string caller = "",
            ConsoleColor prefixColor = ConsoleColor.White,
            ConsoleColor messageColor = ConsoleColor.Gray
        )
        {
            #if DEBUG
                if(!String.IsNullOrEmpty(caller))
                    prefix = caller;
            #endif

            prefix = $" [{prefix}] ";
            int prefixLength = prefix.Length;
            string prefixSpacer = new string(' ', prefixLength);

            Console.ForegroundColor = prefixColor;
            Console.Write(prefix);
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

        static void ResetForegroundColor()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}