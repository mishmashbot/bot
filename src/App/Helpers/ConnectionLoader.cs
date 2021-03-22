using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ollio.Common;
using Ollio.Common.Models;
using Ollio.State;
using ConfigModels = Ollio.Common.Models.Config;
using TelegramBot = Telegram.Bot;
using TelegramBotArgs = Telegram.Bot.Args;
using TelegramBotEnums = Telegram.Bot.Types.Enums;

namespace Ollio.Helpers
{
    public class ConnectionLoader
    {
        public async static Task<bool> CreateConnection(ConfigModels.Bot botConfig)
        {
            var connection = new Connection
            {
                Config = botConfig,
                Id = botConfig.Id,
                Plugins = botConfig.Plugins,
                Token = botConfig.Client.Token
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

            connection.Client = new TelegramBot.TelegramBotClient(connection.Token);
            connection.Task = new Task(async () => await SetHandlers(connection));

            var apiTestResult = await ClientHelpers.TestConnection(connection);

            if (apiTestResult)
            {
                connection.Me = await connection.Client.GetMeAsync();
                Write.Success($"{connection.Id}: Connected as @{connection.Me.Username} ({connection.Me.Id})");
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
                if (!RuntimeState.DryRun)
                {
                    connection.Client.OnMessage += (sender, e) =>
                        Respond(connection, e, (async () => await ClientHandlers.HandleMessage(sender, e, connection)));
                    //connection.Client.OnMessageEdited += async (sender, e) =>
                    //    await TelegramHandlers.HandleMessageEdited(sender, e, connection);

                    await ClientHelpers.InitPlugins(connection);
                }

                await ClientHelpers.SetCommands(connection);

                if(!RuntimeState.DryRun)
                    connection.Client.StartReceiving();
            }
            catch (Exception e)
            {
                Write.Error(e);
            }
        }

        static void Respond(Connection connection, TelegramBotArgs.MessageEventArgs messageEvent, Func<Task> response)
        {
            // may god have mercy on my soul
            Task waitTask = new Task(async () => {
                Task respondTask = new Task(async () => await response());
                
                DateTime taskStartTime = DateTime.Now;
                respondTask.Start();

                while (!respondTask.IsCompleted)
                {
                    if ((DateTime.Now - taskStartTime).Seconds > 0.25)
                        await connection.Client.SendChatActionAsync(messageEvent.Message.Chat.Id, TelegramBotEnums.ChatAction.Typing);
                }
            });

            waitTask.Start();
        }
    }
}