using System;
using System.Threading;
using Microsoft.DotNet.PlatformAbstractions;
using Ollio.Config.Helpers;
using Ollio.Config.State;
using Ollio.Models;
using Ollio.Server.Helpers;
using Ollio.Utilities;

namespace Ollio.Server
{
    class Program
    {
        public static RuntimeInfo RuntimeInfo { get; set; }
        static ManualResetEvent QuitEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            SetRuntimeInfo();

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ConsoleUtilities.PrintStartupMessage();
            Console.CancelKeyPress += (sender, e) => {
                QuitEvent.Set();
                e.Cancel = true;
            };

            try
            {
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
