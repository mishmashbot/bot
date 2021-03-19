using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ollio.Clients;
using Ollio.Common;
using Ollio.Common.Enums;
using Ollio.Common.Models;
using Ollio.State;
using ConfigModels = Ollio.Common.Models.Config;
using TelegramBotArgs = Telegram.Bot.Args;
using TelegramBotTypes = Telegram.Bot.Types;

namespace Ollio.Helpers
{
    public class ConnectionLoader
    {
        public async static Task<bool> CreateConnection(ConfigModels.Bot bot)
        {
            /*List<TelegramBotTypes.User> owners = new List<TelegramBotTypes.User>();

            if (bot.Owners != null)
            {
                foreach (var owner in bot.Owners)
                {
                    TelegramBotTypes.User user = new TelegramBotTypes.User
                    {
                        FirstName = owner.Name,
                        Id = owner.Id
                    };
                    owners.Add(user);
                }
            }*/

            var context = new Context
            {
                Config = bot.Config,
                //Owners = owners,
                Random = Program.Random
            };

            var connection = new Connection
            {
                Context = context,
                Id = bot.Id,
                Plugins = bot.Plugins,
                Token = bot.Config.Token
            };

            return await CreateConnection(connection);
        }

        public async static Task<int> CreateConnections(List<ConfigModels.Bot> bots)
        {
            int count = 0;

            foreach (var bot in bots)
            {
                var success = await CreateConnection(bot);

                if (success)
                    count++;
            }

            return count;
        }

        public async static Task<bool> CreateConnection(Connection connection)
        {
            bool hasConnected = false;
            bool hasStarted = false;

            connection.Client = ClientHelpers.CreateClient(connection);
            connection.Task = new Task(async () => await SetHandlers(connection));

            var apiTestResult = await ClientHelpers.TestConnection(connection);

            if (apiTestResult)
            {
                connection.Context.DateConnected = DateTime.Now;
                connection.Context.Me = await connection.Client.GetMe();
                connection.Context.RuntimeInfo = Program.RuntimeInfo;
                Write.Success($"{connection.Id}: Connected as @{connection.Context.Me.Username} ({connection.Context.Me.Id})");
                hasConnected = true;
            }
            else
            {
                Write.Warning($"{connection.Id}: Unable to connect to Telegram");
            }

            if (hasConnected)
            {
                try
                {
                    connection.Task.Start();
                    Write.Debug($"{connection.Id}: Started as task #{connection.Task.Id}");
                    connection.Context.DateStarted = DateTime.Now;
                    hasStarted = true;
                }
                catch (ThreadStartException e)
                {
                    Write.Error(e);
                    connection.Client = null;
                }
            }

            if (hasConnected && hasStarted)
                return true;
            else
                return false;
        }

        static async Task SetHandlers(Connection connection)
        {
            try
            {
                await ClientHelpers.SetCommands(connection);

                if (!RuntimeState.DryRun)
                {
                    switch(connection.Client.Type)
                    {
                        case ClientType.Telegram:
                            SetTelegramHandlers(connection);
                            break;
                    }

                    await connection.Client.Connect();
                    await PluginLoader.Init(connection);
                }
            }
            catch (Exception e)
            {
                Write.Error(e);
            }
        }

        static void SetTelegramHandlers(Connection connection)
        {
            var telegramClient = (TelegramClient)connection.Client;

            telegramClient.Client.OnMessage += (sender, e) =>
                Respond(connection, e, (async () => await TelegramHandlers.HandleMessage(sender, e, connection)));
            //connection.Client.OnMessageEdited += async (sender, e) =>
            //    await TelegramHandlers.HandleMessageEdited(sender, e, connection);
        }

        static void Respond(Connection connection, TelegramBotArgs.MessageEventArgs messageEvent, Func<Task> response)
        {
            // may god have mercy on my soul
            Task waitTask = new Task(async () => {
                Task respondTask = new Task(async () => await response());
                
                DateTime taskStartTime = DateTime.Now;
                respondTask.Start();

                /*while (!respondTask.IsCompleted)
                {
                    if ((DateTime.Now - taskStartTime).Seconds > 0.5)
                        await connection.Client.ToggleBusy(request);
                }*/
            });

            waitTask.Start();
        }
    }
}