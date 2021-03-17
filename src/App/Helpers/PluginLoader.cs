using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
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

        public static async Task Init(Connection connection)
        {
            PluginEntities.Connection castedConnected = new PluginEntities.Connection
            {
                Client = connection.Client,
                Context = connection.Context,
                Id = connection.Id,
                Plugins = connection.Plugins,
                Task = connection.Task,
                Token = connection.Token
            };

            PluginEntities.Request request = new PluginEntities.Request
            {
                Context = connection.Context
            };

            foreach (var trigger in Plugins)
            {
                var plugin = trigger.Value;

                if (connection.Plugins.Contains(plugin.Id))
                {
                    Write.Debug($"{plugin.Id}: Triggering OnInit()");
                    plugin.OnInit();
                    plugin.OnInit(castedConnected);

                    Write.Debug($"{plugin.Id}: Triggering OnInitAsync()");
                    var onInitTask = plugin.OnInitAsync();
                    var onInitTaskWithConnection = plugin.OnInitAsync(castedConnected);
                    if (onInitTask != null)
                        await onInitTask;
                    if (onInitTaskWithConnection != null)
                        await onInitTaskWithConnection;

                    Task tickTask = new Task(async () =>
                    {
                        // TODO: Use a timer instead
                        //       Need to work out why it doesn't work though: when a command is processed,
                        //        the timer seems to stop
                        int tickRate = 0;

                        try
                        {
                            while (connection != null)
                            {
                                PluginResponse response = null;
                                PluginResponse responseAsync = null;

                                response = plugin.OnTick(request);
                                var responseAsyncTask = plugin.OnTickAsync(request);
                                if (responseAsyncTask != null)
                                    responseAsync = await responseAsyncTask;

                                if (response != null)
                                    Write.Debug(response.Text);

                                if (response != null)
                                    await TelegramHelpers.SendMessage(response, connection);

                                if (responseAsync != null)
                                    await TelegramHelpers.SendMessage(responseAsync, connection);

                                tickRate = 3000;
                                Thread.Sleep(tickRate);
                            }
                        }
                        catch (Exception e)
                        {
                            if (e.Message.Contains("429 (Too Many Requests)"))
                                tickRate = tickRate + 3000;

                            Write.Error(e);
                        }
                    });

                    tickTask.Start();
                }
            }
        }

        public static int UpdatePlugins()
        {
            Plugins = new Dictionary<PluginSubscription, PluginEntities.IPlugin>();
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
                        Plugins.Add(plugin.Subscription, plugin);

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