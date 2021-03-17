using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Figgle;
using Microsoft.DotNet.PlatformAbstractions;
using Ollio.Common;
using Ollio.Common.Enums;
using Ollio.Common.Models;
using Ollio.Helpers;
using Ollio.State;
using Ollio.Utilities;

namespace Ollio
{
    class Program
    {
        public static Random Random { get; set; }
        public static RuntimeInfo RuntimeInfo { get; set; }
        static ManualResetEvent QuitEvent = new ManualResetEvent(false);

        public class Arguments
        {
            /*[Option('c', "config", HelpText = "Location of configuration directory.")]
            public string ConfigDirectory { get; set; }*/
            //[Option('n', "no-update", HelpText = "Stop plugins installing/updating from repository.")]
            //public bool NoUpdate { get; set; }
            [Option("no-compile", HelpText = "Stop compilation of projects in ./plugins.")]
            public bool NoCompile { get; set; }
        }

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            Random = new Random();
            await SetRuntimeInfo();
            await PrintStartupMessage(RuntimeInfo);

            if(PreflightChecks())
                /*await*/ Parser.Default.ParseArguments<Arguments>(args)
                    .WithParsed(Run);
                    //.WithParsedAsync(Run);
            else
                Exit();
        }

        public static void Exit()
        {
            Write.Info($"Exiting...");
            Environment.Exit(Int32.MinValue);
        }

        static void Run(Arguments arguments)
        {
            Console.CancelKeyPress += (sender, e) =>
            {
                QuitEvent.Set();
                e.Cancel = true;
            };

            try
            {
                PrintRuntimeInfo();

                RuntimeState.NoCompilePlugins = arguments.NoCompile;
                //RuntimeState.NoUpdatePlugins = arguments.NoUpdate;

                var pluginsCount = PluginLoader.UpdatePlugins();
                var connectionsCount = ConnectionLoader.CreateConnections(ConfigState.Bots);

                if (pluginsCount == 0)
                {
                    Write.Warning("No plugins found.");
                    Exit();
                }

                if (connectionsCount == 0)
                {
                    Write.Warning("No connections created.");
                    Exit();
                }
            }
            catch (Exception e)
            {
                Write.Error(e);
            }

            QuitEvent.WaitOne();
        }

        static bool PreflightChecks()
        {
            bool success = true;

            bool isFirstTimeLaunch = !(ConfigLoader.UpdateConfig(Path.Combine(RuntimeUtilities.GetConfigRoot(), "config.yaml")));
            bool isSingleFileBinary = RuntimeUtilities.IsSingleFileBinary();

            if (isSingleFileBinary)
            {
                Write.Warning($"Ollio was built as a single-file binary (/p:PublishSingleFile=true), which is not yet supported.");
                success = false;
            }

            if (isFirstTimeLaunch)
            {
                // TODO: Create config
                Write.Warning($"Settings file did not exist. Please edit './config/config.yaml' and re-run."); // TODO: Programatically get config location
                success = false;
            }

            return success;
        }

        static void PrintRuntimeInfo(bool debugOnly = true)
        {
            RuntimeInfo r = RuntimeInfo;
            string runtimeInfoString = $@"AppCommit:       {r.AppCommit}
AppCopyright:    {r.AppCopyright}
AppVersion:      {r.AppVersion}
DateStarted:     {r.DateStarted.ToString("yyyy-MM-dd hh:mm:ss zzz")}
Hostname:        {r.Hostname}
IPAddress:       {r.IPAddress}
OS:              {r.OS}
OSVersion:       {r.OSVersion}
Platform:        {r.Platform}
PlatformVersion: {r.PlatformVersion}
{Environment.NewLine}"; // NOTE: NewLine is for readability

            if(debugOnly)
                Write.Debug(runtimeInfoString);
            else
                Write.Info(runtimeInfoString);
        }

        static async Task PrintStartupMessage(RuntimeInfo runtime, bool printJoke = true)
        {
            string logo = FiggleFonts.Standard.Render("Ollio").TrimEnd();
            string[] logoSplit = logo.Split('\n');
            Version v = runtime.AppVersion;

            string joke = "";
            string version = runtime.GetVersion(includeBuild: true, includeCommit: true, includeRelease: true);

            if(printJoke)
            {
                try {
                    var dadJoke = await new Ollio.Clients.ICanHazDadJoke().Get();
                    joke = dadJoke.Data.Joke.Replace(System.Environment.NewLine, " ");
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

            string commit = "";
            if(informalVersionAttribute?.InformationalVersion != null)
            {
                var informalVersion = informalVersionAttribute.InformationalVersion;
                commit = informalVersion.Substring(informalVersion.IndexOf('+') + 1);
            }

            string os = RuntimeEnvironment.OperatingSystem
                .Replace("alpine", "Alpine")
                .Replace("debian", "Debian")
                .Replace("elementary", "elementaryOS")
                .Replace("ubuntu", "Ubuntu");

            RuntimeInfo.AppCommit = commit;
            RuntimeInfo.AppVersion = Assembly.GetExecutingAssembly().GetName().Version;
            RuntimeInfo.DateStarted = DateTime.Now;
            RuntimeInfo.Hostname = System.Net.Dns.GetHostName();
            RuntimeInfo.IPAddress = wtfIsMyIp.Data.IPAddress;
            RuntimeInfo.OS = os;
            RuntimeInfo.OSVersion = RuntimeEnvironment.OperatingSystemVersion;
            RuntimeInfo.Platform = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
            RuntimeInfo.PlatformVersion = Environment.Version.ToString();
        }
    }
}
