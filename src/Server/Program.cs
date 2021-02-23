using System;
using System.Threading;
using CommandLine;
using Ollio.Plugin;
using Ollio.Server.Helpers;
using Ollio.Utilities;

namespace Ollio.Server
{
    class Program
    {
        static ManualResetEvent QuitEvent = new ManualResetEvent(false);

        public class Options
        {
            //[Option('c', "config-dir", Required = false, HelpText = "Location of configuration directory", Default = "config")]
            //public string ConfigDirectory { get; set; }

            //[Option("plugins-dir", Required = false, HelpText = "Location of plugins directory", Default = "plugins")]
            //public string PluginsDirectory { get; set; }
        }

        static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eArgs) => {
                QuitEvent.Set();
                eArgs.Cancel = true;
            };

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

                SetupContexts();

                /*var request = new PluginRequest
                {
                    RawInput = "hello2"
                };

                var pluginResponse = PluginLoader.HandleRequest(request);

                ConsoleUtilities.PrintDebugMessage(pluginResponse.RawOutput);*/
            }
            catch (Exception e)
            {
                ConsoleUtilities.PrintErrorMessage(e);
            }

            QuitEvent.WaitOne();
        }

        static void ParseConfig()
        {

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

        static void SetupContexts()
        {
            string[] pluginsToLoad = new string[] {
                "ollio.helloworld"
            };

            foreach(string pluginToLoad in pluginsToLoad) {
                PluginBase foundPlugin = PluginLoader.GetPluginById(pluginToLoad);

                if(foundPlugin != null) {
                    PluginLoader.StartupPlugin(pluginToLoad);
                }
            }
        }
    }
}
