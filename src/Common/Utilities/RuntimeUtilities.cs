using System.IO;
using System.Reflection;

namespace Ollio.Utilities
{
    public class RuntimeUtilities
    {
        public static string GetConfigRoot()
        {
            return Path.Combine(
                GetRootDirectory(),
                "config"
            );
        }

        public static string GetPluginsRoot()
        {
#if DEBUG
            return Path.Combine(
                GetRootDirectory(),
                "src",
                "Plugins"
            );
#else
            return Path.Combine(
                GetRootDirectory(),
                "plugins"
            );
#endif
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
    }
}