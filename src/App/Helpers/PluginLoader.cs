using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Ollio.Common;
using Ollio.Common.Models;
using Ollio.Common.Types;
using Ollio.Utilities;
using PluginEntities = Ollio.Plugin;

namespace Ollio.Helpers
{
    public static class PluginLoader
    {
        public static bool NoCompile { get; set; }
        public static Dictionary<PluginSubscription, PluginEntities.IPlugin> Plugins { get; set; }

        public static List<PluginResponse> Invoke(Message message, Context context, Connection connection)
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
                    (context.EventType == EventType.Message && message.Text.StartsWith(connection.Prefix)) ||
                    context.EventType == EventType.Callback
                )
            )
            {
                Regex commandRegex = null;
                Match commandMatch = null;

                switch(context.EventType)
                {
                    /*case EventType.Callback:
                        commandRegex = new Regex("");
                        break;*/
                    case EventType.Message:
                        commandRegex = new Regex(@$"^\/(?<cmd>\w*)(?:$|@{connection.Me.Username.ToLower()}$)");
                        break;
                }

                commandMatch = commandRegex.Match(message.Text);
                if(commandMatch.Success)
                {
                    command = new Command
                    {
                        /*Arguments =,*/
                        Directive = commandMatch.Groups["cmd"].Value.ToLower()
                    };

                    Write.Debug(command.Directive);

                    message.Text = message.Text.Replace(command.Directive, "").Trim();
                    triggers = Plugins
                        .Where(p => p.Key.Commands.Contains(command.Directive));

                    if(triggers != null)
                        isCommand = true;
                }
            }

            if(!isCommand)
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
                        PluginEntities.Request castedRequest = new PluginEntities.Request
                        {
                            Command = command,
                            Runtime = Program.RuntimeInfo,
                            Message = message
                        };

                        PluginResponse response = null;

                        switch(context.EventType) {
                            case EventType.Message:
                                response = plugin.OnMessage(castedRequest);
                                break;
                        }

                        responses.Add(response);
                    }
                }
            }

            return responses;
        }

        // TODO: Not load in plugins we don't need!
        public static int UpdatePlugins()
        {
            Plugins = new Dictionary<PluginSubscription, PluginEntities.IPlugin>();

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
                Write.Success($"Loaded {count} plugins");

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
                //bool built = NoCompile ? RuntimeUtilities.Compile(projectPath) : true;
                bool built = RuntimeUtilities.Compile(projectPath);

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

        static Assembly LoadPlugin(string fullPath)
        {
            string pluginLocation = Path.GetFullPath(fullPath);
            Write.Debug($"Loading plugin: {pluginLocation}");
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }
    }
}