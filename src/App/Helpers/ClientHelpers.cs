using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Ollio.Clients;
using Ollio.Common;
using Ollio.Common.Enums;
using Ollio.Common.Interfaces;
using Ollio.Common.Models;

namespace Ollio.Helpers
{
    public class ClientHelpers
    {
        public static IClient CreateClient(Connection connection)
        {
            return new TelegramClient(connection.Token);
        }

        /*public static async Task<TelegramBotTypes.Message> EditMessage(HistoryMessage historyMessage, PluginResponse response, Connection connection)
        {
            TelegramBotTypes.Message editedMessage = null;

            if (!String.IsNullOrEmpty(response.Text))
                response.Text = ParseTextHtml(response.Text);

            switch(response.Type)
            {
                case MessageType.Text:
                    editedMessage = await connection.Client.EditMessageTextAsync(
                        historyMessage.ChatId,
                        historyMessage.MessageId,
                        response.Text,
                        DefaultParseMode,
                        response.DisableWebpagePreview,
                        null // TODO: Reply markup!
                    );
                    break;
            }

            return editedMessage;
        }*/

        public static async Task SetCommands(Connection connection)
        {
            IDictionary<string, string> commands = new Dictionary<string, string>();

            if (connection.Context.Config.Prefix == '/')
            {
                foreach (var pluginCollection in PluginLoader.Plugins)
                {
                    var plugin = pluginCollection.Value;

                    foreach(var commandCollection in plugin.Subscription.Commands)
                    {
                        var command = commandCollection.Key;
                        var description = commandCollection.Value;

                        if(connection.Plugins.Contains(plugin.Id))
                        {
                            commands.Add($"/{command}", description);
                        }
                    }
                }
            }

            SortedDictionary<string, string> sortedCommands = new SortedDictionary<string,string>(commands);
            await connection.Client.SetCommands(sortedCommands);

#if DEBUG
            foreach (var command in sortedCommands)
            {
                Write.Debug($"Set command ({connection.Context.Me.Id}): {command.Key} - {command.Value}");
            }
#endif
        }

        public static async Task<Message> SendMessage(PluginResponse response, Connection connection)
        {
            Message sentMessage = null;

            if (!String.IsNullOrEmpty(response.Text))
                response.Text = ParseTextHtml(response.Text);

            switch (response.Type)
            {
                case MessageType.Photo:
                    sentMessage = await connection.Client.SendPhotoMessage(response);
                    break;
                case MessageType.Text:
                    sentMessage = await connection.Client.SendTextMessage(response);
                    break;
            }

            return sentMessage;
        }

        public static void SetTelegramHandlers(Connection connection)
        {
            var telegramClient = (TelegramClient)connection.Client;

            //telegramClient.Client.OnMessage += (sender, e) =>
                //Respond(connection, e, (async () => await TelegramHandlers.HandleMessage(sender, e, connection)));
            //connection.Client.OnMessageEdited += async (sender, e) =>
            //    await TelegramHandlers.HandleMessageEdited(sender, e, connection);
        }

        public static async Task<bool> TestConnection(Connection connection, int maxAttempts = 3)
        {
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
                bool result = await connection.Client.TestConnection();
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

        static string ParseTextHtml(string text)
        {
            text = text
                .Replace("<br />", Environment.NewLine)
                .Replace("<hr />", "—")
                .Replace("c>", "code>");

            return text;
        }
    }
}