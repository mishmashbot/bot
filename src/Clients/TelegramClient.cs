using System.Collections.Generic;
using System.Threading.Tasks;
using Ollio.Common.Enums;
using Ollio.Common.Interfaces;
using Ollio.Common.Models;
using TelegramBot = Telegram.Bot;
using TelegramBotEnums = Telegram.Bot.Types.Enums;
using TelegramBotTypes = Telegram.Bot.Types;

namespace Ollio.Clients
{
    public class TelegramClient : IClient
    {
        public TelegramBot.ITelegramBotClient Client { get; set; }
        public ClientType Type { get => ClientType.Telegram; }
        const TelegramBotEnums.ParseMode DefaultParseMode = TelegramBotEnums.ParseMode.Html;

        public TelegramClient(string token)
        {
            Client = new TelegramBot.TelegramBotClient(token);
        }

        public void CreateTelegram(string token)
        {
            Client = new TelegramBot.TelegramBotClient(token);
        }

        public Task Connect()
        {
            return Task.Run(() => Client.StartReceiving());
        }

        public Task Disconnect()
        {
            return Task.Run(() => Client.StopReceiving());
        }

        public async Task<User> GetMe()
        {
            var result = await Client.GetMeAsync();
            return CreateUser(result);
        }

        public async Task SetCommands(IDictionary<string, string> commands)
        {
            List<TelegramBotTypes.BotCommand> botCommands = new List<TelegramBotTypes.BotCommand>();

            foreach(var command in commands)
                botCommands.Add(new TelegramBotTypes.BotCommand {
                    Command = command.Key,
                    Description = command.Value
                });

            await Client.SetMyCommandsAsync(botCommands);
        }

        public async Task<Message> SendPhotoMessage(PluginResponse response)
        {
            var result = await Client.SendPhotoAsync(
                response.ChatId,
                response.File,
                response.Text,
                DefaultParseMode,
                response.DisableNotification,
                response.ReplyToMessageId,
                null // TODO: Reply markup!
            );

            return CreateMessage(result);
        }

        public async Task<Message> SendTextMessage(PluginResponse response)
        {
            var result = await Client.SendTextMessageAsync(
                response.ChatId,
                response.Text,
                DefaultParseMode,
                response.DisableWebpagePreview,
                response.DisableNotification,
                response.ReplyToMessageId,
                null // TODO: Reply markup!
            );

            return CreateMessage(result);
        }

        public async Task<bool> TestConnection()
        {
            return await Client.TestApiAsync();
        }

        public async Task ToggleBusy(PluginRequest request)
        {
            await Client.SendChatActionAsync(request.Message.Chat.Id, TelegramBotEnums.ChatAction.Typing);
        }

        Message CreateMessage(TelegramBotTypes.Message telegramMessage)
        {
            Message message = new Message();

            message.Animation = telegramMessage.Animation;
            message.Audio = telegramMessage.Audio;
            message.AuthorSignature = telegramMessage.AuthorSignature;
            message.Caption = telegramMessage.Caption;
            message.CaptionEntities = telegramMessage.CaptionEntities;
            message.ChannelChatCreated = telegramMessage.ChannelChatCreated;
            message.Chat = telegramMessage.Chat;
            message.ConnectedWebsite = telegramMessage.ConnectedWebsite;
            message.Contact = telegramMessage.Contact;
            message.Date = telegramMessage.Date;
            message.DeleteChatPhoto = telegramMessage.DeleteChatPhoto;
            message.Document = telegramMessage.Document;
            message.EditDate = telegramMessage.EditDate;
            message.Entities = telegramMessage.Entities;
            message.ForwardDate = telegramMessage.ForwardDate;
            message.ForwardFrom = telegramMessage.ForwardFrom;
            message.ForwardFromChat = telegramMessage.ForwardFromChat;
            message.ForwardFromMessageId = telegramMessage.ForwardFromMessageId;
            message.ForwardSenderName = telegramMessage.ForwardSenderName;
            message.ForwardSignature = telegramMessage.ForwardSignature;
            message.From = telegramMessage.From;
            message.Game = telegramMessage.Game;
            message.GroupChatCreated = telegramMessage.GroupChatCreated;
            message.Invoice = telegramMessage.Invoice;
            message.LeftChatMember = telegramMessage.LeftChatMember;
            message.Location = telegramMessage.Location;
            message.MediaGroupId = telegramMessage.MediaGroupId;
            message.MessageId = telegramMessage.MessageId;
            message.MigrateFromChatId = telegramMessage.MigrateFromChatId;
            message.MigrateToChatId = telegramMessage.MigrateToChatId;
            message.NewChatMembers = telegramMessage.NewChatMembers;
            message.NewChatPhoto = telegramMessage.NewChatPhoto;
            message.NewChatTitle = telegramMessage.NewChatTitle;
            message.PassportData = telegramMessage.PassportData;
            message.Photo = telegramMessage.Photo;
            message.PinnedMessage = telegramMessage.PinnedMessage;
            message.Poll = telegramMessage.Poll;
            message.ReplyMarkup = telegramMessage.ReplyMarkup;
            message.ReplyToMessage = telegramMessage.ReplyToMessage;
            message.Sticker = telegramMessage.Sticker;
            message.SuccessfulPayment = telegramMessage.SuccessfulPayment;
            message.SupergroupChatCreated = telegramMessage.SupergroupChatCreated;
            message.Text = telegramMessage.Text;
            message.Venue = telegramMessage.Venue;
            message.ViaBot = telegramMessage.ViaBot;
            message.Video = telegramMessage.Video;
            message.VideoNote = telegramMessage.VideoNote;
            message.Voice = telegramMessage.Voice;

            if(message.ForwardFrom != null) {
                message.IsForwarded = true;
            }

            message.Type = (MessageType)(int)message.Type;

            return message;
        }

        User CreateUser(TelegramBotTypes.User telegramUsername)
        {
            User user = new User();

            user.CanJoinGroups = telegramUsername.CanJoinGroups;
            user.CanReadAllGroupMessages = telegramUsername.CanReadAllGroupMessages;
            user.FirstName = telegramUsername.FirstName;
            user.Id = telegramUsername.Id;
            user.IsBot = telegramUsername.IsBot;
            user.LanguageCode = telegramUsername.LanguageCode;
            user.LastName = telegramUsername.LastName;
            user.SupportsInlineQueries = telegramUsername.SupportsInlineQueries;
            user.Username = telegramUsername.Username;

            return user;
        }
    }
}