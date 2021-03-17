using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ollio.Common;
using Ollio.Common.Enums;
using Ollio.Common.Models;
using Ollio.State;
using Ollio.Utilities;
using PluginEntities = Ollio.Plugin;

namespace Ollio.Helpers
{
    public static class PluginLoader
    {
        public static Dictionary<PluginSubscription, PluginEntities.IPlugin> Plugins { get; set; }

        public static async Task<List<PluginResponse>> Invoke(Message message, Connection connection)
        {
            Command command = null;
            bool isCommand = false;
            List<PluginResponse> responses = new List<PluginResponse>();
            IEnumerable<KeyValuePair<PluginSubscription, PluginEntities.IPlugin>> triggers = null;
            var type = message.Type;

            if (
                message.Type == Message.MessageType.Text &&
                message.Text != null && // NOTE: For safety in case MessageType is Text for some reason
                (
                    (message.EventType == EventType.Message && message.Text.StartsWith(connection.Context.Config.Prefix)) ||
                    message.EventType == EventType.Callback
                )
            )
            {
                Regex commandRegex = new Regex(@$"^(?<cmd>\{connection.Context.Config.Prefix}\w*|\w*:)(?:$|@{connection.Context.Me.Username})?(?:$|\s)?(?<args>.*)");
                Match commandMatch = commandRegex.Match(message.Text);

                if (commandMatch.Success)
                {
                    var argsGroup = commandMatch.Groups["args"];
                    var cmdGroup = commandMatch.Groups["cmd"];

                    command = new Command
                    {
                        Argument = (argsGroup != null) ? argsGroup.Value : "",
                        Directive = (cmdGroup != null) ? cmdGroup.Value.ToLower().Replace(connection.Context.Config.Prefix.ToString(), "") : "" // NOTE: This should never be null, but it looks nicer
                    };

                    switch (message.EventType)
                    {
                        case EventType.Callback:
                            command.Arguments = command.Argument.Split(":");
                            triggers = Plugins
                                .Where(p => p.Key.Callbacks.Contains(command.Directive));
                            break;
                        case EventType.Message:
                            command.Arguments = command.Argument.Split(" ");
                            triggers = Plugins
                                .Where(p => p.Key.Commands.ContainsKey(command.Directive));
                            break;
                    }

                    if (triggers != null)
                        isCommand = true;
                }
            }

            if (!isCommand)
            {
                switch (type)
                {
                    case Message.MessageType.Audio:
                        triggers = Plugins.Where(p => p.Key.OnAudio == true);
                        break;
                    case Message.MessageType.Document:
                        triggers = Plugins.Where(p => p.Key.OnDocument == true);
                        break;
                    case Message.MessageType.Photo:
                        triggers = Plugins.Where(p => p.Key.OnPhoto == true);
                        break;
                    case Message.MessageType.Sticker:
                        triggers = Plugins.Where(p => p.Key.OnSticker == true);
                        break;
                    case Message.MessageType.Text:
                        triggers = Plugins.Where(p => p.Key.OnText == true);
                        break;
                }
            }

            if (triggers != null)
            {
                foreach (var trigger in triggers)
                {
                    PluginEntities.IPlugin plugin = trigger.Value; // NOTE: For readability
                    bool isValidPlugin = connection.Plugins.Contains(plugin.Id);

                    if (isValidPlugin)
                    {
                        PluginResponse response = null;
                        PluginResponse responseAsync = null;

                        PluginEntities.Request request = new PluginEntities.Request
                        {
                            Command = command,
                            Context = connection.Context,
                            Runtime = Program.RuntimeInfo,
                            Message = message
                        };

                        switch (message.EventType)
                        {
                            case EventType.Message:
                                // INVESTIGATE: Is firing both of these slow?
                                response = plugin.OnMessage(request);
                                var responseAsyncTask = plugin.OnMessageAsync(request);
                                if (responseAsyncTask != null)
                                    responseAsync = await responseAsyncTask;
                                break;
                        }

                        if (response != null)
                            responses.Add(response);

                        if (responseAsync != null)
                            responses.Add(responseAsync);
                    }
                }
            }

            return responses;
        }

        public static int UpdatePlugins()
        {
            Plugins = new Dictionary<PluginSubscription, PluginEntities.IPlugin>();
            List<string> pluginsToLoad = new List<string>();
            int count = 0;

            string[] pluginPaths = CollectPluginPaths().ToArray();

            if(pluginPaths.Count() > 0)
            {
                var loadedPlugins = pluginPaths.SelectMany(pluginPath =>
                {
                    Assembly pluginAssembly = LoadPlugin(pluginPath);
                    return CreatePlugin(pluginAssembly);
                }).ToList();

                // TODO: Do this in the above method to improve speed
                foreach (var bot in ConfigState.Bots)
                    foreach (var botPlugin in bot.Plugins)
                        pluginsToLoad.Add(botPlugin);

                foreach (var plugin in loadedPlugins)
                    if (pluginsToLoad.Contains(plugin.Id) && plugin.Subscription != null)
                        Plugins.Add(plugin.Subscription, plugin);

                count = Plugins.Count();
            }

            return count;
        }

        static IEnumerable<PluginEntities.IPlugin> CreatePlugin(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(PluginEntities.IPlugin).IsAssignableFrom(type))
                {
                    PluginEntities.IPlugin result = Activator.CreateInstance(type) as PluginEntities.IPlugin;
                    if (result != null)
                    {
                        yield return result;
                    }
                }
            }
        }

        static List<string> CollectPluginPaths()
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
                ).Length > 0 &&
                !RuntimeState.NoCompilePlugins
            )
            {
                string projectPath = Path.Combine(RuntimeUtilities.GetPluginsRoot(), pluginName);
                string platform = RuntimeUtilities.GetTargetFrameworkForProject(Path.Combine(projectPath, $"{pluginName}.csproj"));
                bool built = RuntimeUtilities.Compile(projectPath);

#if DEBUG
                string configuration = "Debug";
#else
                string configuration = "Release";
#endif

                if (built)
                    file = Path.Combine(projectPath, "bin", configuration, platform, $"{pluginName}.dll");
            }
            else
            {
                file = Path.Combine(RuntimeUtilities.GetPluginsRoot(), $"{pluginName}.dll");
            }

            if (File.Exists(file))
                return file;
            else
                return null;
        }

        static Assembly LoadPlugin(string fullPath)
        {
            Assembly assembly = null;

            try
            {
                string pluginLocation = Path.GetFullPath(fullPath);
                PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);

                Write.Debug($"Loading plugin: {pluginLocation}");
                assembly = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
            }
            catch (Exception e)
            {
                Write.Error(e);
                Program.Exit();
            }

            return assembly;
        }
    }
}