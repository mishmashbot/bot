using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ollio.Common;
using Ollio.Common.Models;
using Ollio.State;
using Ollio.Utilities;
using TelegramBotArgs = Telegram.Bot.Args;

namespace Ollio.Helpers
{
    public class ClientHandlers
    {
        public static async Task HandleMessage(object sender, TelegramBotArgs.MessageEventArgs messageEvent, Connection connection)
        {
            try
            {
                List<HistoryMessage> historyMessages = new List<HistoryMessage>();
                Message message = new Message();
                message.CreateMessage(messageEvent);

                List<PluginResponse> responses = await PluginLoader.Invoke(message, connection);

                if (responses != null)
                {
                    foreach (var response in responses)
                    {
                        if (response.ChatId == 0)
                            response.ChatId = messageEvent.Message.Chat.Id;

                        var messageResult = await ClientHelpers.SendMessage(response, connection);
                        historyMessages.Add(new HistoryMessage {
                            ChatId = messageResult.Chat.Id,
                            MessageId = messageResult.MessageId
                        });
                    }

                    MessageHistory.Add(messageEvent.Message.MessageId, historyMessages);
                }
            }
            catch (Exception e)
            {
                await HandleError(e, messageEvent, connection);
            }
        }

        static async Task HandleError(Exception exception, TelegramBotArgs.MessageEventArgs messageEvent, Connection connection)
        {
            bool throwError = true;
            Guid reference = Guid.NewGuid();

            if(exception.Message.Contains("message must be non-empty"))
                throwError = false;

            if(throwError)
            {
                Write.Error(exception, reference);

                string remark = StringUtilities.SelectRandomString(
                    new List<String> {
                        "Abort, Retry, Fail?",
                        "Enhance your calm",
                        "FUBAR",
                        "Have you tried turning it off and on again?",
                        "It's not supposed to do that",
                        "lp0 on fire",
                        "Not a typewriter",
                        "PEBCAK",
                        "She's dead, Jim!",
                        "сука блят"
                    }
                );

                if (messageEvent != null && connection != null)
                {
                    PluginResponse response = new PluginResponse
                    {
                        ChatId = messageEvent.Message.Chat.Id,
                        Text = $@"🚫 {exception.Message}<br /><c>{reference}</c><br /><hr /><br /><b>{remark}</b>"
                    };

                    await ClientHelpers.SendMessage(response, connection);
                };
            }
        }

        //public static async Task HandleMessageEdited(object sender, MessageEventArgs messageEvent, Connection connection)
        //{
            /*try
            {
                var testGuid = Guid.NewGuid();
                Message message = new Message();
                message.CreateMessage(messageEvent, connection);

                var update = MessageHistory.Update(messageEvent.Message.MessageId);
                if(update != null)
                {
                    List<PluginResponse> responses = await PluginLoader.Invoke(message, connection);

                    foreach(var historyMessage in update.Messages)
                    {
                        //var editedResult = await TelegramHelpers.EditMessage(historyMessage, )
                        case Message.MessageType.Text:
                            await connection.Client.EditMessageTextAsync(
                                historyMessage.ChatId, historyMessage.MessageId,
                                $"Changed: {testGuid}",
                                DefaultParseMode
                            );
                            break;
                    }
                }
            } 
            catch(Exception e)
            {
                await HandleError(e, messageEvent, connection);
            }*/
        //}
    }
}