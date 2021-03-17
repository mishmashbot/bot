using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Ollio.Common;
using Ollio.Common.Models;
using ConfigModels = Ollio.Common.Models.Config;
using TelegramBotTypes = Telegram.Bot.Types;

namespace Ollio.Helpers
{
    public class ConnectionLoader
    {
        public static bool CreateConnection(ConfigModels.Bot bot)
        {
            List<TelegramBotTypes.User> owners = new List<TelegramBotTypes.User>();

            if(bot.Owners != null)
            {
                foreach(var owner in bot.Owners)
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

            return CreateConnection(connection);
        }

        public static int CreateConnections(List<ConfigModels.Bot> bots)
        {
            int count = 0;

            foreach (var bot in bots)
            {
                var success = CreateConnection(bot);

                if(success)
                    count++;
            }

            return count;
        }

        public static bool CreateConnection(Connection connection)
        {
            connection.Client = new TelegramBotClient(connection.Token);

            if (connection.Client.TestApiAsync().Result)
            {
                connection.Thread = new Thread(async () => await StartConnection(connection))
                {
                    Name = connection.Id
                };

                try
                {
                    connection.Thread.Start();

                    connection.Context.DateStarted = DateTime.Now;
                    connection.Context.Me = connection.Client.GetMeAsync().Result;
                    
                    Write.Success($"{connection.Id}: Connected as @{connection.Context.Me.Username} ({connection.Context.Me.Id})");
                    return true;
                }
                catch (ThreadStartException e)
                {
                    Write.Error(e);
                }
            }
            else
            {
                Write.Warning($"{connection.Id}: Unable to connect to Telegram");
            }

            return false;
        }

        static async Task StartConnection(Connection connection)
        {
            try
            {
                await TelegramHelpers.SetCommands(connection);

                connection.Client.OnMessage += async (sender, e) =>
                    await TelegramHandlers.HandleMessage(sender, e, connection);
                connection.Client.OnMessageEdited += async (sender, e) =>
                    await TelegramHandlers.HandleMessageEdited(sender, e, connection);

                connection.Client.StartReceiving();
            }
            catch (Exception e)
            {
                Write.Error(e);
            }
        }
    }
}