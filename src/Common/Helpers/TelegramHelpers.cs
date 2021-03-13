using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Telegram.Bot.Args;
using Ollio.Models;
using Ollio.Utilities;
using TelegramBotEnums = Telegram.Bot.Types.Enums;

namespace Ollio.Helpers
{
    public class TelegramHelpers
    {
        public async Task HandleUpdate(MessageEventArgs messageEvent, Connection connection)
        {

        }

        public async Task<Message> HandleMessage(MessageEventArgs messageEvent, Connection connection)
        {
            Message message = ParseMessageEvent(messageEvent);

            return message;
        }

        public async Task HandleMessageEdited(MessageEventArgs messageEvent, Connection connection)
        {

        }

        Message ParseMessageEvent(MessageEventArgs messageEvent)
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

        /*FileStream fileStream = File.Open("/tmp/perfect.mp4", FileMode.Open);
        Stream stream = fileStream;
        MessageFile file = new MessageFile(stream);
        var message = new Message();
        message.CreateVideo(file);
        //message.CreateText("it werks");
        ConsoleUtilities.PrintDebugMessage(message.Text);
        await connection.Client.SendVideoAsync(
            //-1001127490424,
            -1001105619215,
            message.File
        );
        fileStream.Close();*/
        /*await connection.Client.SendTextMessageAsync(
            -1001127490424,
            message.Text
        );*/
    }
}