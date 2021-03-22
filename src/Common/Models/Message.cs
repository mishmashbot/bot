using Ollio.Common.Enums;
using TelegramBotArgs = Telegram.Bot.Args;
using TelegramBotTypes = Telegram.Bot.Types;

namespace Ollio.Common.Models
{
    public class Message : TelegramBotTypes.Message
    {
        public EventType EventType { get; set; } = EventType.Message;
        public new bool IsForwarded { get; set; }
        public new MessageType Type { get; set; }

        public void CreateMessage(TelegramBotArgs.MessageEventArgs messageEvent)
        {
            var m = messageEvent.Message;

            Animation = m.Animation;
            Audio = m.Audio;
            AuthorSignature = m.AuthorSignature;
            Caption = m.Caption;
            CaptionEntities = m.CaptionEntities;
            ChannelChatCreated = m.ChannelChatCreated;
            Chat = m.Chat;
            ConnectedWebsite = m.ConnectedWebsite;
            Contact = m.Contact;
            Date = m.Date;
            DeleteChatPhoto = m.DeleteChatPhoto;
            Document = m.Document;
            EditDate = m.EditDate;
            Entities = m.Entities;
            ForwardDate = m.ForwardDate;
            ForwardFrom = m.ForwardFrom;
            ForwardFromChat = m.ForwardFromChat;
            ForwardFromMessageId = m.ForwardFromMessageId;
            ForwardSenderName = m.ForwardSenderName;
            ForwardSignature = m.ForwardSignature;
            From = m.From;
            Game = m.Game;
            GroupChatCreated = m.GroupChatCreated;
            Invoice = m.Invoice;
            LeftChatMember = m.LeftChatMember;
            Location = m.Location;
            MediaGroupId = m.MediaGroupId;
            MessageId = m.MessageId;
            MigrateFromChatId = m.MigrateFromChatId;
            MigrateToChatId = m.MigrateToChatId;
            NewChatMembers = m.NewChatMembers;
            NewChatPhoto = m.NewChatPhoto;
            NewChatTitle = m.NewChatTitle;
            PassportData = m.PassportData;
            Photo = m.Photo;
            PinnedMessage = m.PinnedMessage;
            Poll = m.Poll;
            ReplyMarkup = m.ReplyMarkup;
            ReplyToMessage = m.ReplyToMessage;
            Sticker = m.Sticker;
            SuccessfulPayment = m.SuccessfulPayment;
            SupergroupChatCreated = m.SupergroupChatCreated;
            Text = m.Text;
            Venue = m.Venue;
            ViaBot = m.ViaBot;
            Video = m.Video;
            VideoNote = m.VideoNote;
            Voice = m.Voice;

            if(ForwardFrom != null) {
                IsForwarded = true;
            }

            Type = (MessageType)(int)m.Type;
        }
    }
}