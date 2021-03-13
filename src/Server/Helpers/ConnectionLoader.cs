using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Ollio.Models;
using Ollio.Types;
using Ollio.Utilities;
using ConfigModels = Ollio.Config.Models;
using TelegramBotEnums = Telegram.Bot.Types.Enums;

namespace Ollio.Server.Helpers
{
    public class ConnectionLoader
    {
        public static void CreateConnection(ConfigModels.Context context)
        {
            var connection = new Connection
            {
                Name = context.Id,
                Token = context.Config.Token
            };

            CreateConnection(connection);
        }

        public static void CreateConnection(Connection connection)
        {
            if (String.IsNullOrEmpty(connection.Name))
            {
                connection.Name = $"unknown_{connection.Token}";
            }

            connection.Client = new TelegramBotClient(connection.Token);

            if (connection.Client.TestApiAsync().Result)
            {
                connection.Thread = new Thread(
                    async () => await StartConnection(connection)
                )
                {
                    Name = connection.Name
                };
                
                connection.Thread.Start(); // TODO: Handle thread failure
                ConsoleUtilities.PrintSuccessMessage($"{connection.Name}: Connected to Telegram (#{connection.Thread.ManagedThreadId})");
            }
            else
            {
                ConsoleUtilities.PrintWarningMessage($"{connection.Name}: Unable to connect to Telegram");
            }
        }

        public static void CreateConnections(List<ConfigModels.Context> contexts)
        {
            foreach (var context in contexts)
            {
                CreateConnection(context);
            }
        }

        static async Task StartConnection(Connection connection)
        {
            try
            {
                connection.Client.OnMessage += async (sender, e) =>
                {
                    Context context = GetContext(connection, EventType.Message);
                    Message message = ParseMessageEvent(e);
                    List<PluginResponse> responses = PluginLoader.Invoke(message, context, connection);

                    if (responses != null)
                    {
                        foreach (var response in responses)
                        {
                            await connection.Client.SendTextMessageAsync(
                                -1001127490424,
                                response.RawOutput
                            );
                        }
                    }
                };

                //connection.Client.OnMessageEdited += async (sender, e) => { await _telegramHelpers.HandleMessageEdited(e, connection); };
                /*connection.Client.OnMessage += async (sender, e) => {
                    ConsoleUtilities.PrintDebugMessage("Got message!");
                    await connection.Client.SendTextMessageAsync(
                        -1001127490424,
                        $"You said {e.Message.Text}. This is from thread {connection.Thread.ManagedThreadId}."
                    );
                };*/

                connection.Client.StartReceiving();
            }
            catch (Exception e)
            {
                ConsoleUtilities.PrintErrorMessage(e);
            }
        }

        static Context GetContext(Connection connection, EventType eventType)
        {
            Context context = new Context();

            context.EventType = eventType;

            return context;
        }

        static Message ParseMessageEvent(MessageEventArgs messageEvent)
        {
            Message message = new Message();

            var payload = messageEvent.Message;
            TelegramBotEnums.MessageType type = messageEvent.Message.Type;

            switch (type)
            {
                //case TelegramBotEnums.MessageType.Animation:
                //    break;

                case TelegramBotEnums.MessageType.Audio:
                    message.CreateAudio(
                        new MessageFile(payload.Audio.FileId),
                        payload.Caption,
                        payload.Audio.Duration,
                        payload.Audio.Performer,
                        payload.Audio.Title
                    );
                    break;

                /*case TelegramBotEnums.MessageType.Contact:
                    break;*/

                /*case TelegramBotEnums.MessageType.Dice:
                    break;*/

                case TelegramBotEnums.MessageType.Document:
                    message.CreateDocument(
                        new MessageFile(payload.Document.FileId),
                        payload.Caption
                    );
                    break;

                /*case TelegramBotEnums.MessageType.Game:
                    break;*/

                /*case TelegramBotEnums.MessageType.Invoice:
                    break;*/

                /*case TelegramBotEnums.MessageType.Location:
                    break;*/

                case TelegramBotEnums.MessageType.Photo:
                    message.CreatePhoto(
                        payload.Photo.ToList().Select(p =>
                            new MessageFile(p.FileId)
                        ).ToList(),
                        payload.Caption
                    );
                    break;

                /*case TelegramBotEnums.MessageType.Poll:
                    break;*/

                case TelegramBotEnums.MessageType.Sticker:
                    message.CreateSticker(
                        new MessageFile(payload.Sticker.FileId)
                    );
                    break;

                case TelegramBotEnums.MessageType.Text:
                    message.CreateText(
                        payload.Text
                    );
                    break;

                /*case TelegramBotEnums.MessageType.Venue:
                    break;*/

                case TelegramBotEnums.MessageType.Video:
                    message.CreateVideo(
                        new MessageFile(payload.Video.FileId),
                        payload.Caption,
                        payload.Video.Duration,
                        payload.Video.Height,
                        payload.Video.Width
                    );
                    break;

                    /*case TelegramBotEnums.MessageType.VideoNote
                        break;*/

                    /*case TelegramBotEnums.MessageType.Voice:
                        break;*/
            }

            return message;
        }
    }
}