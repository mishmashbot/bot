using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Ollio.Utilities
{
    public class RuntimeUtilities
    {
        public static bool IsCompilingAvailable { get; set; } = true;

        public static bool Compile(string path)
        {
            bool built = false;

            if (!IsCompilingAvailable)
            {
                return false;
            }

            var projectFiles = Directory.GetFiles(path, "*.csproj");

            if (projectFiles.Length != 0)
            {
                var project = projectFiles.ToList().FirstOrDefault();

                try
                {
                    var process = new Process()
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "dotnet",
                            Arguments = $"build {path}",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                        }
                    };

                    process.Start();
                    ConsoleUtilities.PrintInfoMessage($"Comping project: {project}");
                    string result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    if (result.Contains("Build FAILED."))
                    {
                        ConsoleUtilities.PrintWarningMessage(@$"Compilation failed ({project})
{result}");
                    } else {
                        built = true;
                    }
                }
                catch (Win32Exception)
                {
                    IsCompilingAvailable = false;
                    ConsoleUtilities.PrintWarningMessage(".NET SDK is unavailable. Compilation is disabled.");
                }
            }

            return built;
        }

        public static string GetConfigRoot()
        {
            return Path.Combine(
                GetRootDirectory(),
                "config"
            );
        }

        public static string GetPluginsRoot()
        {
            return Path.Combine(
                GetRootDirectory(),
                "plugins"
            );
        }

        public static string GetRootDirectory()
        {
            var binaryLocation = Process.GetCurrentProcess().MainModule.FileName;
#if DEBUG
            return Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(
                                    Path.GetDirectoryName(binaryLocation))))))));
#else
            return Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(binaryLocation)
            ));
#endif
        }

        public static string GetTargetFrameworkForProject(string path)
        {
            string platform = "net6.0";

            if (File.Exists(path) && path.Contains(".csproj"))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(path);
                XmlNodeList xmlRoot = xmlDocument.SelectNodes("Project");

                platform = ((XmlElement)((XmlElement)xmlRoot[0])
                    .GetElementsByTagName("PropertyGroup")[0])
                    .GetElementsByTagName("TargetFramework")[0]
                    .InnerText; // TODO: Make this more safe
            }

            return platform;
        }
    }
}