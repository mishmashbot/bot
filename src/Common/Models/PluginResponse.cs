using System;
using System.IO;
using Ollio.Common.Enums;

namespace Ollio.Common.Models
{
    public class PluginResponse
    {
        public bool DisableNotification { get; set; }
        public bool DisableWebpagePreview { get; set; }
        public long ChatId { get; set; }
        public UploadFile File { get; set; }
        public int ReplyToMessageId { get; set; }
        public bool Silent { get; set; }
        public string Text { get; set; }
        public MessageType Type { get; set; } = MessageType.Text;

        public void CreatePhotoResponse(
            UploadFile file,
            string caption = "",
            int replyToMessageId = 0,
            long chatId = 0,
            bool disableWebpagePreview = false,
            bool disableNotification = false
        )
        {
            ChatId = chatId;
            DisableNotification = disableNotification;
            DisableWebpagePreview = disableWebpagePreview;
            File = file;
            ReplyToMessageId = replyToMessageId;
            Text = caption;
            Type = MessageType.Photo;
        }

        public void CreatePhotoResponse(
            Stream fileStream,
            string caption = "",
            int replyToMessageId = 0,
            long chatId = 0,
            bool disableWebpagePreview = false,
            bool disableNotification = false
        )
        {
            CreatePhotoResponse(
                fileStream,
                caption,
                replyToMessageId,
                chatId,
                disableWebpagePreview,
                disableNotification
            );
        }

        public void CreatePhotoResponse(
            string url,
            string caption = "",
            int replyToMessageId = 0,
            long chatId = 0,
            bool disableWebpagePreview = false,
            bool disableNotification = false
        )
        {
            CreatePhotoResponse(
                new UploadFile(new Uri(url)),
                caption,
                replyToMessageId,
                chatId,
                disableWebpagePreview,
                disableNotification
            );
        }

        public void CreateTextResponse(
            string text,
            int replyToMessageId = 0,
            long chatId = 0,
            bool disableWebpagePreview = false,
            bool disableNotification = false
        )
        {
            ChatId = chatId;
            DisableNotification = disableNotification;
            DisableWebpagePreview = disableWebpagePreview;
            ReplyToMessageId = replyToMessageId;
            Type = MessageType.Text;
            Text = text;
        }
    }
}