using System;
using CommandLine;
using Ollio.Plugin;
using Ollio.Server.Helpers;
using Ollio.Utilities;

namespace Ollio.Server
{
    class Program
    {
        public class Options
        {
            [Option('c', "config-dir", Required = false, HelpText = "Location of configuration directory", Default = "config")]
            public string ConfigDirectory { get; set; }

            //[Option("plugins-dir", Required = false, HelpText = "Location of plugins directory", Default = "plugins")]
            //public string PluginsDirectory { get; set; }
        }

        static void Main(string[] args)
        {
            try
            {
                ParseCommandLineArguments(args);
                Console.OutputEncoding = System.Text.Encoding.UTF8;

                ConsoleUtilities.PrintStartupMessage();

                var pluginsCount = PluginLoader.UpdatePlugins();
                var commandsCount = PluginLoader.UpdatePluginCommands();

                if (commandsCount == 0)
                {
                    // TODO: Handle no commands
                }

                ConsoleUtilities.PrintSuccessMessage($"Loaded {pluginsCount} plugins with {commandsCount} commands");

                var request = new PluginRequest
                {
                    RawInput = "hello2"
                };

                var pluginResponse = PluginLoader.InvokePlugin(request);

                ConsoleUtilities.PrintDebugMessage(pluginResponse.RawOutput);
            }
            catch (Exception e)
            {
                ConsoleUtilities.PrintErrorMessage(e);
            }
        }

        static void ParseCommandLineArguments(string[] arguments)
        {
            Parser.Default.ParseArguments<Options>(arguments)
                .WithParsed<Options>(o =>
                {
                    //AppArguments.ConfigDirectory = o.ConfigDirectory
                    //    .Replace("\\", "/"); // TODO: Parse this safer
                });
        }
    }
}
