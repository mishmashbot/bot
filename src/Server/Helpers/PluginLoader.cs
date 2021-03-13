using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Ollio.Models;
using Ollio.Plugin;
using Ollio.Types;
using Ollio.Utilities;

namespace Ollio.Server.Helpers
{
    // TODO: WASM plugins?
    //       Lua plugins?
    //       PowerShell plugins?
    public static class PluginLoader
    {
        public static bool NoCompile { get; set; }
        public static Dictionary<PluginSubscription, IPlugin> Plugins { get; set; }

        public static List<PluginResponse> Invoke(Message message, Context context, Connection connection)
        {
            string command = "";
            bool isCommand = false;
            List<PluginResponse> responses = new List<PluginResponse>();
            IEnumerable<KeyValuePair<PluginSubscription, IPlugin>> triggers = null;
            var type = message.Type;

            if (
                message.Type == Message.MessageType.Text &&
                (
                    (context.EventType == EventType.Message && message.Text.StartsWith("/")) ||
                    context.EventType == EventType.Callback
                )
            )
            {
                isCommand = true;
                command = "info";
            }

            if (isCommand)
            {
                message.Text = message.Text.Replace(command, "").Trim();

                switch(context.EventType)
                {
                    case EventType.Callback:
                        triggers = Plugins
                            .Where(s => s.Key.Callbacks.Contains(command));
                        break;
                    case EventType.Message:
                        triggers = Plugins
                            .Where(s => s.Key.Commands.Contains(command));
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case Message.MessageType.Audio:
                        triggers = Plugins.Where(s => s.Key.OnAudio == true);
                        break;
                    case Message.MessageType.Document:
                        triggers = Plugins.Where(s => s.Key.OnDocument == true);
                        break;
                    case Message.MessageType.Photo:
                        triggers = Plugins.Where(s => s.Key.OnPhoto == true);
                        break;
                    case Message.MessageType.Sticker:
                        triggers = Plugins.Where(s => s.Key.OnSticker == true);
                        break;
                    case Message.MessageType.Text:
                        triggers = Plugins.Where(s => s.Key.OnText == true);
                        break;
                }
            }

            if (triggers != null)
            {
                foreach (var trigger in triggers)
                {
                    IPlugin plugin = trigger.Value; // NOTE: For readability
                    //bool isValidPlugin = connection.Plugins.Contains(plugin.Id);
                    bool isValidPlugin = true;

                    if (isValidPlugin)
                    {
                        Request castedRequest = new Request
                        {
                            Command = command,
                            Runtime = Program.RuntimeInfo,
                            Message = message
                        };

                        PluginResponse response = null;

                        switch(context.EventType) {
                            case EventType.Message:
                                plugin.OnMessage(castedRequest);
                                break;
                        }

                        responses.Add(response);
                    }
                }
            }

            return responses;
        }

        public static int UpdatePlugins()
        {
            Plugins = new Dictionary<PluginSubscription, IPlugin>();

            string[] pluginPaths = CollectPlugins().ToArray();

            var loadedPlugins = pluginPaths.SelectMany(pluginPath =>
            {
                Assembly pluginAssembly = LoadPlugin(pluginPath);
                return CreatePlugin(pluginAssembly);
            }).ToList();

            foreach (var plugin in loadedPlugins)
            {
                if (plugin.Subscription != null)
                {
                    Plugins.Add(plugin.Subscription, plugin);
                }
            }

            int count = Plugins.Count();

            if(count > 0)
                ConsoleUtilities.PrintSuccessMessage($"Loaded {count} plugins");

            return count;
        }

        static List<string> CollectPlugins()
        {
            var pluginPaths = new List<string>();

            IEnumerable<string> plugins = Directory
                .GetDirectories(RuntimeUtilities.GetPluginsRoot())
                .Select(d => Path.GetFileName(d));

            foreach (var plugin in plugins)
            {
                string pluginPath = GetPluginPath(plugin);
                if (!String.IsNullOrEmpty(pluginPath))
                    pluginPaths.Add(pluginPath);
            }

            return pluginPaths;
        }

        static string GetPluginPath(string pluginName)
        {
            string file = "";

            if (
                Directory.GetFiles(
                    Path.Combine(RuntimeUtilities.GetPluginsRoot(), pluginName),
                    "*.csproj"
                ).Length > 0
            )
            {
                string projectPath = Path.Combine(RuntimeUtilities.GetPluginsRoot(), pluginName);
                string platform = RuntimeUtilities.GetTargetFrameworkForProject(Path.Combine(projectPath, $"{pluginName}.csproj"));
                bool built = NoCompile ? RuntimeUtilities.Compile(projectPath) : true;
                
                if(built)
                    file = Path.Combine(projectPath, "bin", "Debug", platform, $"{pluginName}.dll");
            }
            else
            {
                file = Path.Combine(RuntimeUtilities.GetPluginsRoot(), pluginName, $"{pluginName}.dll");
            }

            if (File.Exists(file))
                return file;
            else
                return null;
        }

        static IEnumerable<IPlugin> CreatePlugin(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IPlugin).IsAssignableFrom(type))
                {
                    IPlugin result = Activator.CreateInstance(type) as IPlugin;
                    if (result != null)
                    {
                        yield return result;
                    }
                }
            }
        }

        static Assembly LoadPlugin(string fullPath)
        {
            string pluginLocation = Path.GetFullPath(fullPath);
            ConsoleUtilities.PrintDebugMessage($"Loading plugin: {pluginLocation}");
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }
    }
}