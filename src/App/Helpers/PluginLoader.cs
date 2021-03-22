using System;
using System.Collections.Generic;
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
        static List<PluginEntities.IPlugin> Plugins { get; set; }

        public static async Task<List<PluginResponse>> Invoke(Message message, Connection connection)
        {
            Command command = null;
            IEnumerable<PluginEntities.IPlugin> foundPlugins = null;
            List<PluginResponse> responses = new List<PluginResponse>();

            if (
                message.Type == MessageType.Text &&
                message.Text != null && // NOTE: For safety in case MessageType is Text for some reason
                (
                    (message.EventType == EventType.Message && message.Text.StartsWith(connection.Config.Client.Prefix)) ||
                    message.EventType == EventType.Callback
                )
            )
            {
                Regex commandRegex = new Regex(@$"^(?<cmd>\{connection.Config.Client.Prefix}\w*|\w*:)(?:$|@{connection.Me.Username})?(?:$|\s)?(?<args>.*)");
                Match commandMatch = commandRegex.Match(message.Text);

                if (commandMatch.Success)
                {
                    var argsGroup = commandMatch.Groups["args"];
                    var cmdGroup = commandMatch.Groups["cmd"];

                    command = new Command
                    {
                        Argument = (argsGroup != null) ? argsGroup.Value : "",
                        Directive = (cmdGroup != null) ? cmdGroup.Value.ToLower().Replace(connection.Config.Client.Prefix.ToString(), "") : "" // NOTE: This should never be null, but it looks nicer
                    };

                    switch (message.EventType)
                    {
                        case EventType.Callback:
                            command.Arguments = command.Argument.Split(":");
                            foundPlugins = Plugins
                                .Where(p => p.Subscription.Callbacks.Contains(command.Directive));
                            break;
                        case EventType.Message:
                            command.Arguments = command.Argument.Split(" ");
                            foundPlugins = Plugins
                                .Where(p => p.Subscription.Commands.ContainsKey(command.Directive));
                            break;
                    }
                }
            }
            else
            {
                switch (message.Type)
                {
                    case MessageType.Audio:
                        foundPlugins = Plugins.Where(p => p.Subscription.OnAudio == true);
                        break;
                    case MessageType.Document:
                        foundPlugins = Plugins.Where(p => p.Subscription.OnDocument == true);
                        break;
                    case MessageType.Photo:
                        foundPlugins = Plugins.Where(p => p.Subscription.OnPhoto == true);
                        break;
                    case MessageType.Sticker:
                        foundPlugins = Plugins.Where(p => p.Subscription.OnSticker == true);
                        break;
                    case MessageType.Text:
                        foundPlugins = Plugins.Where(p => p.Subscription.OnText == true);
                        break;
                }
            }

            if (foundPlugins != null)
            {
                foreach (var plugin in foundPlugins)
                {
                    bool isValidPlugin = connection.Plugins.Contains(plugin.Id);

                    if (isValidPlugin)
                    {
                        PluginResponse response = null;
                        PluginResponse responseAsync = null;

                        PluginEntities.Request request = new PluginEntities.Request
                        {
                            Command = command,
                            Message = message,
                            Me = connection.Me,
                            RuntimeInfo = Program.RuntimeInfo
                        };

                        switch (message.EventType)
                        {
                            case EventType.Message:
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

        public static async Task Init(Connection connection)
        {
            PluginEntities.Connection castedConnection = new PluginEntities.Connection
            {
                Client = connection.Client,
                Id = connection.Id,
                Plugins = connection.Plugins,
                Task = connection.Task,
                Token = connection.Token
            };

            foreach (var plugin in Plugins)
            {
                if(connection.Plugins.Contains(plugin.Id))
                {
                    plugin.OnInit();
                    plugin.OnInit(castedConnection);
                    var onInitAsync = plugin.OnInitAsync(castedConnection);
                    if (onInitAsync != null)
                        await onInitAsync;
                }
            }

        }

        public static List<PluginEntities.IPlugin> GetPlugins()
        {
            return Plugins;
        }

        public static int UpdatePlugins()
        {
            Plugins = new List<PluginEntities.IPlugin>();
            List<string> pluginsToLoad = new List<string>();
            int count = 0;

            string[] pluginPaths = CollectPluginPaths().ToArray();

            if (pluginPaths.Count() > 0)
            {
                var loadedPlugins = pluginPaths.SelectMany(pluginPath =>
                {
                    return CreatePlugin(pluginPath);
                }).ToList();

                // TODO: Do this in the above method to improve speed
                foreach (var bot in ConfigState.Bots)
                    foreach (var botPlugin in bot.Plugins)
                        pluginsToLoad.Add(botPlugin);

                foreach (var plugin in loadedPlugins)
                    if (pluginsToLoad.Contains(plugin.Id) && plugin.Subscription != null)
                        Plugins.Add(plugin);

                count = Plugins.Count();
            }

            return count;
        }

        // TODO: Handle duplicate plugin IDs
        static List<string> CollectPluginPaths()
        {
            var pluginPaths = new List<string>();
            List<string> searchPaths = new List<string>();

            List<string> foundDirectories = Directory
                .GetDirectories(RuntimeUtilities.GetPluginsRoot())
                .Select(d => Path.GetFileName(d)).ToList();

            if (foundDirectories != null)
                searchPaths.AddRange(foundDirectories);

            foreach (var searchPath in searchPaths)
            {
                string pluginPath = GetPluginPath(searchPath);
                if (!String.IsNullOrEmpty(pluginPath))
                    pluginPaths.Add(pluginPath);
            }

            return pluginPaths;
        }

        // TODO: Refactor because its shit
        static string GetPluginPath(string pluginName)
        {
            string searchPath = Path.Combine(RuntimeUtilities.GetPluginsRoot(), pluginName);
            string foundPluginPath = "";

            if (
                Directory.GetFiles(searchPath, "*.csproj").Length > 0 &&
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
                    foundPluginPath = Path.Combine(projectPath, "bin", configuration, platform, $"{pluginName}.dll");
            }
            else
            {
                foundPluginPath = Path.Combine(RuntimeUtilities.GetPluginsRoot(), pluginName, $"{pluginName}.dll");
            }

            if (File.Exists(foundPluginPath))
                return foundPluginPath;
            else
                return null;
        }

        static IEnumerable<PluginEntities.IPlugin> CreatePlugin(string fullPath)
        {
            Assembly assembly = null;
            string pluginLocation = Path.GetFullPath(fullPath);

            Write.Debug($"Creating plugin: {pluginLocation}");

            try
            {
                PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
                var assemblyName = new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation));
                assembly = loadContext.LoadFromAssemblyName(assemblyName);
            }
            catch (Exception e)
            {
                Write.Error(e);
                Program.Exit();
            }

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
    }
}