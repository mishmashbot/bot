using System.IO;
using System.Reflection;

namespace Mishmash.Utilities
{
    public class RuntimeUtilities
    {
        public static string GetPluginsRoot()
        {
#if DEBUG
            // ./Plugins/
            return Path.Combine(
                GetRootDirectory(),
                "Plugins"
            );
#else
            // ./plugins/
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
                                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)))))));
#else
            return Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            ));
#endif
        }
    }
}