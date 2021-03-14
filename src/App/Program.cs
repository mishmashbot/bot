using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Figgle;
using Microsoft.DotNet.PlatformAbstractions;
using Ollio.Common;
using Ollio.Common.Models;
using Ollio.Common.Types;
using Ollio.Config;
using Ollio.Helpers;
using Ollio.Utilities;

namespace Ollio
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
        }

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            SetRuntimeInfo();

            await Parser.Default.ParseArguments<Arguments>(args)
                .WithParsedAsync(Run);
        }

        public static void Exit(ExitStatus status = ExitStatus.Unknown)
        {
            Write.Info("Exiting...");
            int exitCode = (int)status;
            Environment.Exit(exitCode);
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
                PrintStartupMessage(RuntimeInfo);

                bool firstTimeLaunch = !(ConfigLoader.UpdateConfig(Path.Combine(RuntimeUtilities.GetConfigRoot(), "config.yaml")));
                if (firstTimeLaunch)
                {
                    Exit(ExitStatus.FirstTimeLaunch);
                }

                //if(!arguments.NoCompile)
                //    PluginLoader.NoCompile = true;

                if (!arguments.NoJoke)
                {
                    var dadJokeClient = new Ollio.Clients.ICanHazDadJoke();
                    var dadJoke = await dadJokeClient.Get();
                    Write.Info(dadJoke.Joke);
                }

                var pluginsCount = PluginLoader.UpdatePlugins();

                if (pluginsCount == 0)
                {
                    Write.Warning("No plugins found.");
                    Exit(ExitStatus.NoPluginsFound);
                }

                ConnectionLoader.CreateConnections(ConfigState.Current.Bots);
            }
            catch (Exception e)
            {
                Write.Error(e);
            }

            QuitEvent.WaitOne();
        }

        static void PrintStartupMessage(RuntimeInfo runtime)
        {
            string logo = FiggleFonts.Standard.Render("Ollio").TrimEnd();
            string[] logoSplit = logo.Split('\n');

            var v = runtime.Version;
            var version = $"{v.Major}.{v.Minor}.{v.Build}";

            if(runtime.VersionCommit != null)
                version += $"+{runtime.VersionCommit}";

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(logo);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string('=', (logoSplit[logoSplit.Length - 2].Length + 1)));
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.Write($" Version ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(version);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($" on ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(runtime.Platform);
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine($" Copyright (C) 2021 ???");
            Console.WriteLine("");
            Write.Reset();
        }

        static void SetRuntimeInfo()
        {
            RuntimeInfo = new RuntimeInfo();

            var informalVersionAttribute = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            var commit = "";
            if(informalVersionAttribute?.InformationalVersion != null)
            {
                var informalVersion = informalVersionAttribute.InformationalVersion;
                commit = informalVersion.Substring(informalVersion.IndexOf('+') + 1);
            }

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
            RuntimeInfo.Version = Assembly.GetExecutingAssembly().GetName().Version;
            RuntimeInfo.VersionCommit = commit;
        }
    }
}
