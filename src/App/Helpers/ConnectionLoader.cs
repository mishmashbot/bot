using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Ollio.Common;
using Ollio.Common.Models;
using Ollio.Common.Types;
using Ollio.Utilities;
using ConfigModels = Ollio.Config.Models;

namespace Ollio.Helpers
{
    public class ConnectionLoader
    {
        public static bool CreateConnection(ConfigModels.Bot bot)
        {
            var connection = new Connection
            {
                Name = bot.Id,
                Plugins = bot.Plugins,
                Prefix = bot.Config.Prefix,
                Token = bot.Config.Token
            };

            return CreateConnection(connection);
        }

        public static bool CreateConnection(Connection connection)
        {
            if (String.IsNullOrEmpty(connection.Name))
            {
                connection.Name = $"unknown_{connection.Token}";
            }

            connection.Client = new TelegramBotClient(connection.Token);

            if (connection.Client.TestApiAsync().Result)
            {
                connection.Thread = new Thread(async () => await StartConnection(connection))
                {
                    Name = connection.Name
                };

                try
                {
                    connection.Thread.Start();

                    var me = connection.Client.GetMeAsync().Result;
                    connection.Me = new User
                    {
                        Id = me.Id,
                        Username = me.Username
                    };

                    Write.Success($"{connection.Name}: Connected as @{connection.Me.Username} ({connection.Me.Id})");
                    return true;
                }
                catch (ThreadStartException e)
                {
                    Write.Error(e);
                }
            }
            else
            {
                Write.Warning($"{connection.Name}: Unable to connect to Telegram");
            }

            return false;
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

        static async Task StartConnection(Connection connection)
        {
            try
            {
                connection.Client.OnMessage += async (sender, e) =>
                    await TelegramHelpers.HandleMessage(sender, e, connection);
                //connection.Client.OnMessageEdited += async (sender, e) =>
                //    await TelegramHelpers.HandleMessageEdit(sender, e, connection);

                connection.Client.StartReceiving();
            }
            catch (Exception e)
            {
                Write.Error(e);
            }
        }
    }
}