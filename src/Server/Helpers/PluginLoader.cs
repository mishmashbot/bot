using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Telegram.Bot;
using Ollio.Models;
using Ollio.Models.Requests;
using Ollio.Models.Responses;
using Ollio.Utilities;

namespace Ollio.Server.Helpers
{
    // TODO: WASM plugins?
    //       Lua plugins?
    //       PowerShell plugins?
    public static class PluginLoader
    {
        public static IDictionary<string, string> Commands { get; set; }
        public static IEnumerable<PluginBase> Plugins { get; set; }

        public static PluginBase GetPluginById(string pluginId)
        {
            var foundPlugins = Plugins.Where(p => p.Id == pluginId);
            var foundPluginsAmount = foundPlugins.Count();

            if(foundPluginsAmount > 1) {
                ConsoleUtilities.PrintWarningMessage($"Found more than one plugin with the ID '{pluginId}'");
            } else if(foundPluginsAmount == 0) {
                ConsoleUtilities.PrintWarningMessage($"Unable to find a plugin with the ID '{pluginId}'");
            }

            return foundPlugins.FirstOrDefault();
        }

        public static PluginResponse HandleRequest(PluginRequest request)
        {
            PluginResponse response = null;
            string pluginId;
            
            if(Commands.TryGetValue(request.RawInput, out pluginId)) {
                PluginBase plugin = GetPluginById(pluginId);
                response = plugin.Invoke(request);
            }

            return response;
        }

        public static void StartupPlugin(string pluginId, dynamic Connection)
        {
            PluginBase plugin = GetPluginById(pluginId);

            if(plugin != null) {
                plugin.Startup();
                plugin.Startup(Connection);
                // TODO: Is there some nice way we can not fire unneeded ones?
                //       Is there a performance hit from doing this?
                //plugin.Startup(Client.DiscordContext);
                //plugin.Startup(Client.IRCContext);
                //plugin.Startup(Client.TelegramContext);
            } else {
                ConsoleUtilities.PrintWarningMessage($"Unable to start '{pluginId}' as it was not found");
            }
        }

        public static int UpdatePlugins()
        {
            string[] pluginPaths = GetPluginPaths().ToArray();

            Plugins = pluginPaths.SelectMany(pluginPath =>
            {
                Assembly pluginAssembly = LoadPlugin(pluginPath);
                return CreateCommands(pluginAssembly);
            }).ToList();

            return Plugins.Count();
        }

        public static int UpdatePluginCommands()
        {
            Commands = new Dictionary<string, string>();

            foreach(var plugin in Plugins)
            {
                foreach(var command in plugin.Commands) {
                    Commands.Add(command, plugin.Id);
                }
            }

            return Commands.Count();
        }

        static string CreatePluginPath(string pluginName)
        {
#if DEBUG
            return Path.Combine(RuntimeUtilities.GetPluginsRoot(), pluginName, "bin", "Debug", "net5", $"{pluginName}.dll"); // TODO: Automate the "bin/Debug/net5/ path somehow?
#else
            return Path.Combine(RuntimeUtilities.GetPluginsRoot(), pluginName, $"{pluginName}.dll");
#endif
        }

        static List<string> GetPluginPaths()
        {
            List<string> pluginPaths = new List<string>();

            var pluginNames = Directory
                .GetDirectories(RuntimeUtilities.GetPluginsRoot())
                .Select(d => Path.GetFileName(d));

            foreach(string pluginName in pluginNames)
            {
                string pluginPath = CreatePluginPath(pluginName);
                if(File.Exists(pluginPath))
                {
                    pluginPaths.Add(pluginPath);
                }
            }

            return pluginPaths;
        }

        static Assembly LoadPlugin(string fullPath)
        {
            string pluginLocation = Path.GetFullPath(fullPath);
            ConsoleUtilities.PrintDebugMessage($"Loading plugin: {pluginLocation}");
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }

        static IEnumerable<PluginBase> CreateCommands(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(PluginBase).IsAssignableFrom(type))
                {
                    PluginBase result = Activator.CreateInstance(type) as PluginBase;
                    if (result != null)
                    {
                        count++;
                        yield return result;
                    }
                }
            }

            if (count == 0)
                ConsoleUtilities.PrintWarningMessage($"{assembly.GetName().Name} ({assembly.GetName().Version}) has no commands to load");
        }
    }
}