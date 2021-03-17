using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ollio.Common;
using Ollio.Common.Models;
using TelegramBotEnums = Telegram.Bot.Types.Enums;
using TelegramBotTypes = Telegram.Bot.Types;

namespace Ollio.Helpers
{
    public class TelegramHelpers
    {
        const TelegramBotEnums.ParseMode DefaultParseMode = TelegramBotEnums.ParseMode.Html;

        public static async Task SetCommands(Connection connection)
        {
            List<TelegramBotTypes.BotCommand> commands = new List<TelegramBotTypes.BotCommand>();

            if (connection.Context.Config.Prefix == '/')
            {
                foreach (var pluginCollection in PluginLoader.Plugins)
                {
                    if (connection.Plugins.Contains(pluginCollection.Value.Id))
                    {
                        foreach (var command in pluginCollection.Value.Subscription.Commands)
                        {
                            var description = command.Value;

                            if (command.Value.Length < 3) // NOTE: Telegram requires descriptions to be more than 3 characters
                                description = "(No description)";

                            TelegramBotTypes.BotCommand botCommand = new TelegramBotTypes.BotCommand
                            {
                                Command = $"/{command.Key}",
                                Description = description
                            };

                            commands.Add(botCommand);
                        }
                    }
                }
            }

            // TODO: Sort list alphabetically

            await connection.Client.SetMyCommandsAsync(commands.AsEnumerable());

#if DEBUG
            var setCommands = await connection.Client.GetMyCommandsAsync();

            foreach (var setCommand in setCommands)
            {
                Write.Debug($"Set command ({connection.Context.Me.Id}): {setCommand.Command} - {setCommand.Description}");
            }
#endif
        }

        public static async Task<TelegramBotTypes.Message> EditMessage(HistoryMessage historyMessage, PluginResponse response, Connection connection)
        {
            TelegramBotTypes.Message editedMessage = null;

            if (!String.IsNullOrEmpty(response.Text))
                response.Text = ParseTextHtml(response.Text);

            switch(response.MessageType)
            {
                case Message.MessageType.Text:
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
        }

        public static async Task<TelegramBotTypes.Message> SendMessage(PluginResponse response, Connection connection)
        {
            TelegramBotTypes.Message sentMessage = null;

            if (!String.IsNullOrEmpty(response.Text))
                response.Text = ParseTextHtml(response.Text);

            switch (response.MessageType)
            {
                case Message.MessageType.Text:
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