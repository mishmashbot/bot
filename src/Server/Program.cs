using System;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Microsoft.DotNet.PlatformAbstractions;
using Ollio.Config;
using Ollio.Models;
using Ollio.Server.Helpers;
using Ollio.Utilities;

namespace Ollio.Server
{
    class Program
    {
        public static RuntimeInfo RuntimeInfo { get; set; }
        static ManualResetEvent QuitEvent = new ManualResetEvent(false);

        public class Arguments
        {
            /*[Option('c', "config", HelpText = "Location of configuration directory.")]
            public string ConfigDirectory { get; set; }*/
            [Option('n', "no-update", HelpText = "Stop plugins installing/updating from repository.")]
            public bool NoUpdate { get; set; }
            [Option("no-compile", HelpText = "Stop compilation of projects in ./plugins.")]
            public bool NoCompile { get; set; }
            [Option("no-joke", HelpText = "Be boring and supress the dad joke.")]
            public bool NoJoke { get; set; }
            [Option("no-logo", HelpText = "Supress the startup logo")]
            public bool NoLogo { get; set; }
        }

        static async Task Main(string[] args)
        {
            SetRuntimeInfo();
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            await Parser.Default.ParseArguments<Arguments>(args)
                .WithParsedAsync(Run);
        }

        static async Task Run(Arguments arguments)
        {
            Console.CancelKeyPress += (sender, e) =>
            {
                QuitEvent.Set();
                e.Cancel = true;
            };

            try
            {
                if(!arguments.NoCompile)
                    PluginLoader.NoCompile = true;

                if(!arguments.NoLogo)
                    ConsoleUtilities.PrintStartupMessage(RuntimeInfo);

                if (!arguments.NoJoke)
                {
                    var dadJokeClient = new Ollio.Client.ICanHazDadJoke();
                    var dadJoke = await dadJokeClient.Get();
                    ConsoleUtilities.PrintInfoMessage(dadJoke.Joke);
                }

                ConfigLoader.UpdateConfig();

                var pluginsCount = PluginLoader.UpdatePlugins();

                if (pluginsCount == 0)
                {
                    ConsoleUtilities.PrintWarningMessage("No plugins found. Exiting...");
                    Environment.Exit(int.MinValue);
                }

                ConnectionLoader.CreateConnections(ConfigState.Current.Contexts);
            }
            catch (Exception e)
            {
                ConsoleUtilities.PrintErrorMessage(e);
            }

            QuitEvent.WaitOne();
        }

        static void PrintHelp<T>(ParserResult<T> result)
        {
            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.AdditionalNewLineAfterOption = false;
                h.Heading = "Myapp 2.0.0-beta"; //fchange header
                h.Copyright = "Copyright (c) 2019 Global.com"; //change copyright text
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);
            Console.WriteLine(helpText);
            Environment.Exit(0);
        }

        static void SetRuntimeInfo()
        {
            RuntimeInfo = new RuntimeInfo();

            var os = RuntimeEnvironment.OperatingSystem
                .Replace("alpine", "Alpine")
                .Replace("debian", "Debian")
                .Replace("elementary", "elementaryOS")
                .Replace("ubuntu", "Ubuntu");

            RuntimeInfo.Hostname = System.Net.Dns.GetHostName();
            RuntimeInfo.OS = os;
            RuntimeInfo.OSVersion = RuntimeEnvironment.OperatingSystemVersion;
            RuntimeInfo.Platform = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
            RuntimeInfo.PlatformVersion = Environment.Version.ToString();
            RuntimeInfo.TimeStarted = DateTime.Now;
        }
    }
}
