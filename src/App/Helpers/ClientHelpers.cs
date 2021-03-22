using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Ollio.Common;
using Ollio.Common.Enums;
using Ollio.Common.Models;
using TelegramBotEnums = Telegram.Bot.Types.Enums;
using TelegramBotTypes = Telegram.Bot.Types;

namespace Ollio.Helpers
{
    public class ClientHelpers
    {
        const TelegramBotEnums.ParseMode DefaultParseMode = TelegramBotEnums.ParseMode.Html;

        public static async Task InitPlugins(Connection connection)
        {
            await PluginLoader.Init(connection);   
        }

        public static async Task<TelegramBotTypes.Message> SendMessage(PluginResponse response, Connection connection)
        {
            TelegramBotTypes.Message sentMessage = null;

            if (!String.IsNullOrEmpty(response.Text))
                response.Text = ParseTextHtml(response.Text);

            switch (response.Type)
            {
                case MessageType.Photo:
                    sentMessage = await connection.Client.SendPhotoAsync(
                        response.ChatId,
                        response.File,
                        response.Text,
                        DefaultParseMode,
                        response.DisableNotification,
                        response.ReplyToMessageId,
                        null // TODO: Reply markup!
                    );
                    break;
                case MessageType.Text:
                    sentMessage = await connection.Client.SendTextMessageAsync(
                        response.ChatId,
                        response.Text,
                        DefaultParseMode,
                        response.DisableWebpagePreview,
                        response.DisableNotification,
                        response.ReplyToMessageId,
                        null // TODO: Reply markup!
                    );
                    break;
            }

            return sentMessage;
        }

        public static async Task SetCommands(Connection connection)
        {
            //IDictionary<string, string> commands = new Dictionary<string, string>();
            List<TelegramBotTypes.BotCommand> botCommands = new List<TelegramBotTypes.BotCommand>();

            if (connection.Instance.Config.Client.Prefix == '/')
            {
                foreach (var plugin in PluginLoader.GetPlugins())
                {
                    foreach(var commandCollection in plugin.Subscription.Commands)
                    {
                        var command = commandCollection.Key;
                        var description = commandCollection.Value;

                        if(connection.Plugins.Contains(plugin.Id))
                        {
                            botCommands.Add(new TelegramBotTypes.BotCommand {
                                Command = $"/{command}",
                                Description = description
                            });
                        }
                    }
                }
            }

            await connection.Client.SetMyCommandsAsync(botCommands);

#if DEBUG
            foreach (var command in botCommands)
            {
                Write.Debug($"Set command ({connection.Instance.Me.Id}): {command.Command} - {command.Description}");
            }
#endif
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