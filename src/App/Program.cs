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
            await SetRuntimeInfo();

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
                await PrintStartupMessage(RuntimeInfo, !arguments.NoJoke);

                bool firstTimeLaunch = !(ConfigLoader.UpdateConfig(Path.Combine(RuntimeUtilities.GetConfigRoot(), "config.yaml")));
                if (firstTimeLaunch)
                {
                    Exit(ExitStatus.FirstTimeLaunch);
                }

                //if(!arguments.NoCompile)
                //    PluginLoader.NoCompile = true;

                var pluginsCount = PluginLoader.UpdatePlugins();

                if (pluginsCount == 0)
                {
                    Write.Warning("No plugins found.");
                    Exit(ExitStatus.NoPluginsFound);
                }

                var connectionsCount = ConnectionLoader.CreateConnections(ConfigState.Current.Bots);

                if (connectionsCount == 0)
                {
                    Write.Warning("No connections created.");
                    Exit(ExitStatus.NoConnectionsCreated);
                }
            }
            catch (Exception e)
            {
                Write.Error(e);
            }

            QuitEvent.WaitOne();
        }

        static async Task PrintStartupMessage(RuntimeInfo runtime, bool printJoke = true)
        {
            string logo = FiggleFonts.Standard.Render("Ollio").TrimEnd();
            string[] logoSplit = logo.Split('\n');
            var v = runtime.AppVersion;

            var joke = "";
            var version = runtime.GetVersion(includeBuild: true, includeCommit: true, includeRelease: true);

            if(printJoke)
            {
                try {
                    var dadJoke = await new Ollio.Clients.ICanHazDadJoke().Get();
                    joke = dadJoke.Joke.Replace(System.Environment.NewLine, " ");
                } catch(Exception) {
                    printJoke = false;
                }
            }

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

            if(printJoke)
                Console.WriteLine($" {joke}");
        
            Console.WriteLine("");
            Write.Reset();
        }

        static async Task SetRuntimeInfo()
        {
            RuntimeInfo = new RuntimeInfo();

            var informalVersionAttribute = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            var wtfIsMyIp = await new Ollio.Clients.WtfIsMyIp().Get();

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

            RuntimeInfo.AppCommit = commit;
            RuntimeInfo.AppVersion = Assembly.GetExecutingAssembly().GetName().Version;
            RuntimeInfo.Hostname = System.Net.Dns.GetHostName();
            RuntimeInfo.IPAddress = wtfIsMyIp.IPAddress;
            RuntimeInfo.OS = os;
            RuntimeInfo.OSVersion = RuntimeEnvironment.OperatingSystemVersion;
            RuntimeInfo.Platform = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
            RuntimeInfo.PlatformVersion = Environment.Version.ToString();
            RuntimeInfo.TimeStarted = DateTime.Now;
        }
    }
}
