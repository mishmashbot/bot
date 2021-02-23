using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Ollio.Config.Helpers;
using Ollio.Config.State;
using Ollio.Plugin;
using Ollio.Server.Helpers;
using Ollio.Utilities;

namespace Ollio.Server
{
    class Program
    {
        static List<TelegramBotClient> Connections = new List<TelegramBotClient>();
        static ManualResetEvent QuitEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ConsoleUtilities.PrintStartupMessage();
            Console.CancelKeyPress += (sender, eArgs) => {
                QuitEvent.Set();
                eArgs.Cancel = true;
            };

            try
            {
                ConfigLoader.UpdateConfig();

                ConsoleUtilities.PrintDebugMessage(ConfigState.Current.Owner.Telegram);

                var pluginsCount = PluginLoader.UpdatePlugins();
                var commandsCount = PluginLoader.UpdatePluginCommands();

                if (commandsCount == 0)
                {
                    // TODO: Handle no commands
                }

                ConsoleUtilities.PrintSuccessMessage($"Loaded {pluginsCount} plugins with {commandsCount} commands");

                //SetupClients();

                /*var request = new PluginRequest
                {
                    RawInput = "hello2"
                };

                var pluginResponse = PluginLoader.HandleRequest(request);

                ConsoleUtilities.PrintDebugMessage(pluginResponse.RawOutput);*/
            }
            catch (Exception e)
            {
                ConsoleUtilities.PrintErrorMessage(e);
            }

            QuitEvent.WaitOne();
        }

        static void SetupClients()
        {
            string[] clients = new string[] {
                "",
                ""
            };

            foreach(string client in clients) {
                var connection = new TelegramBotClient(client);
                if(connection.TestApiAsync().Result) {
                    Connections.Add(connection);
                }
            }

            foreach(var connection in Connections) {
                string[] pluginsToLoad = new string[] {
                    "ollio.helloworld"
                };

                foreach(string pluginToLoad in pluginsToLoad) {
                    PluginBase foundPlugin = PluginLoader.GetPluginById(pluginToLoad);

                    if(foundPlugin != null) {
                        //PluginLoader.StartupPlugin(pluginToLoad, connection.Client);
                    }
                }

                // switch(connection.Service) {
                // case Connection.Services.Telegram:
                connection.OnMessage += HandleMessage;
            }
        }

        static void HandleMessage(object sender, MessageEventArgs telegramMessageEvent)
        {
            Task.Run(() =>
            {

            });
        }
    }

    public class ChatContext<T>
    {
        public T Client { get; set; } = default(T);
    }
}
