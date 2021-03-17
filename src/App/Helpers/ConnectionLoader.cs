using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Ollio.Common;
using Ollio.Common.Models;
using Ollio.State;
using ConfigModels = Ollio.Common.Models.Config;
using TelegramBotTypes = Telegram.Bot.Types;
using System.Diagnostics;

namespace Ollio.Helpers
{
    public class ConnectionLoader
    {
        public async static Task<bool> CreateConnection(ConfigModels.Bot bot)
        {
            List<TelegramBotTypes.User> owners = new List<TelegramBotTypes.User>();

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
            }

            var context = new Context
            {
                Config = bot.Config,
                Owners = owners
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

            connection.Client = new TelegramBotClient(connection.Token);
            connection.Thread = new Thread(async () => await SetHandlers(connection))
            {
                Name = connection.Id
            };

            var apiTestResult = await TestConnection(connection);

            if (apiTestResult)
            {
                connection.Context.DateConnected = DateTime.Now;
                connection.Context.Me = await connection.Client.GetMeAsync();
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
                    connection.Thread.Start();
                    Write.Debug($"{connection.Id}: Started on thread #{connection.Thread.ManagedThreadId}");
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
                await TelegramHelpers.SetCommands(connection);

                if (!RuntimeState.DryRun)
                {
                    connection.Client.OnMessage += async (sender, e) =>
                        await TelegramHandlers.HandleMessage(sender, e, connection);
                    connection.Client.OnMessageEdited += async (sender, e) =>
                        await TelegramHandlers.HandleMessageEdited(sender, e, connection);

                    connection.Client.StartReceiving();
                }
            }
            catch (Exception e)
            {
                Write.Error(e);
            }
        }

        static async Task<bool> TestConnection(Connection connection)
        {
            const int maxAttempts = 3;
            int attempts = 0;
            Stopwatch attemptStopwatch = new Stopwatch();
            bool success = false;

            // TODO: Allow user to Ctrl+C this
            while(!success && attempts != maxAttempts)
            {
                string attemptMessage = $"{connection.Id}: Testing connectivity to Telegram";
                if(attempts > 0)
                    attemptMessage += $" (attempt {attempts+1})...";
                else
                    attemptMessage += "...";
                Write.Debug(attemptMessage);

                attemptStopwatch.Start();
                bool result = await connection.Client.TestApiAsync();
                attemptStopwatch.Stop();
                attemptStopwatch.Reset();

                if(result)
                {
                    success = true;
                    break;
                }
                else
                {
                    attempts++;
                    Thread.Sleep(attemptStopwatch.Elapsed.Milliseconds);
                }
            }

            return success;
        }
    }
}