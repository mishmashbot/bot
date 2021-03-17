using Ollio.Common.Enums;
using TelegramBotArgs = Telegram.Bot.Args;
using TelegramBotEnums = Telegram.Bot.Types.Enums;
using TelegramBotTypes = Telegram.Bot.Types;

namespace Ollio.Common.Models
{
    public class Message : TelegramBotTypes.Message
    {
        public EventType EventType { get; set; } = EventType.Message;
        public new bool IsForwarded { get; set; }
        public new MessageType Type { get; set; }

        public enum MessageType {
            Audio = TelegramBotEnums.MessageType.Audio,
            Document = TelegramBotEnums.MessageType.Document,
            Photo = TelegramBotEnums.MessageType.Photo,
            Sticker = TelegramBotEnums.MessageType.Sticker,
            Text = TelegramBotEnums.MessageType.Text
        }

        public void CreateMessage(TelegramBotArgs.MessageEventArgs messageEvent, Connection connection)
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

        /*public int Duration { get; set; }
        public List<MessageFile> Files { get; set; }
        public int Height { get; set; }
        public MessageOptions Options { get; set; }
        public string Performer { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
        public MessageType Type { get; set; } = MessageType.Text;
        public int Width { get; set; }

        public void CreateMessage(TelegramBotArgs.MessageEventArgs messageEvent)
        {
            var m = messageEvent.Message;

            switch(m.Type)
            {
                case TelegramBotEnums.MessageType.Audio:
                    Duration = m.Audio.Duration;
                    Files = new List<MessageFile> { new MessageFile(m.Audio.FileId) };
                    Performer = m.Audio.Performer;
                    Title = m.Audio.Title;
                    Type = MessageType.Audio;
                    break;
                case TelegramBotEnums.MessageType.Text:
                    Text = m.Text;
                    Type = MessageType.Text;
                    break;
            }
        }

        public void CreateAudio(
            MessageFile audio,
            string caption = default,
            int duration = default,
            string performer = default,
            string title = default,
            MessageOptions options = default
        )
        {
            Duration = duration;
            Files = new List<MessageFile> { audio };
            Performer = performer;
            Text = caption;
            Title = title;
            Type = MessageType.Audio;
        }

        public void CreateDocument(
            MessageFile document,
            string caption
        )
        {
            Files = new List<MessageFile> { document };
            Text = caption;
            Type = MessageType.Document;
        }

        public void CreatePhoto(
            List<MessageFile> photos,
            string caption = default,
            MessageOptions options = default
        )
        {
            Files = photos;
            Options = options;
            Text = caption;
            Type = MessageType.Photo;
        }
        
        public void CreateSticker(
            MessageFile sticker,
            MessageOptions options = default
        )
        {
            Files = new List<MessageFile> { sticker };
            Options = options;
            Type = MessageType.Sticker;
        }

        public void CreateText(
            string text,
            MessageOptions options = default
        )
        {
            Options = options;
            Text = text;
            Type = MessageType.Text;
        }

        public void CreateVideo(
            MessageFile video,
            string caption = default,
            int duration = default,
            int height = default,
            int width = default,
            MessageOptions options = default
        )
        {
            Duration = duration;
            Files = new List<MessageFile> { video };
            Height = height;
            Options = options;
            Text = caption;
            Type = MessageType.Text;
            Width = width;
        }

        public enum MessageType {
            Audio,
            Document,
            Photo,
            Sticker,
            Text
        }*/
    }
}