using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Ollio.Common;

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
                    Write.Info($"Compiling project: {project}");
                    string result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    if (result.Contains("Build FAILED."))
                    {
                        Write.Warning(@$"Compilation failed ({project})
{result}");
                    } else {
                        built = true;
                    }
                }
                catch (Win32Exception)
                {
                    IsCompilingAvailable = false;
                    Write.Warning(".NET SDK is unavailable. Compilation has been disabled");
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
            var binaryLocation = AppContext.BaseDirectory;

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

        public static bool IsSingleFileBinary()
        {
            // TODO: Unfortunately, single-file binaries don't seem to work yet, so we need to
            //        check if this was built as a single-file binary (/p:PublishSingleFile=true)
            //       When Ollio tries to load up plugins, it throws this error:
            //        [Oops] System.InvalidOperationException: Cannot load hostpolicy library. AssemblyDependencyResolver is currently only supported if the runtime is hosted through hostpolicy library.
            //        ---> System.EntryPointNotFoundException: Unable to find an entry point named 'corehost_set_error_writer' in shared library 'libhostpolicy'.
            //        at Interop.HostPolicy.corehost_set_error_writer(IntPtr errorWriter)
            //        at System.Runtime.Loader.AssemblyDependencyResolver..ctor(String componentAssemblyPath)

            // TODO: Better way of checking if this is a single-file binary
#if DEBUG
            return false;
#else
            var root = GetRootDirectory();
            var testFile = Path.Combine(GetRootDirectory(), "Telegram.Bot.dll");

            if(File.Exists(testFile))
                return false;
            else
                return true;
#endif
        }
    }
}