using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Ollio.Models;

namespace Ollio.Utilities
{
    public class RuntimeUtilities
    {
        public static void BuildProject(string path)
        {
            var projectFiles = Directory.GetFiles(path, "*.csproj");

            if (projectFiles.Length != 0)
            {
                var project = projectFiles.ToList().FirstOrDefault();

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

                ConsoleUtilities.PrintDebugMessage($"Building project: {project}");
                process.Start();
                string result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }
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
#if DEBUG
            return Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(
                                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))))))));
#else
            return Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            ));
#endif
        }

        public static string GetTargetFrameworkForProject(string path)
        {
            string platform = "net6.0";

            if(File.Exists(path) && path.Contains(".csproj"))
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