using System;
using System.Diagnostics;

namespace Ollio.Common.Models
{
    public class PluginResponse
    {
        public bool DisableNotification { get; set; }
        public bool DisableWebpagePreview { get; set; }
        public long ChatId { get; set; }
        public Message.MessageType MessageType { get; set; } = Message.MessageType.Text;
        public int ReplyToMessageId { get; set; }
        public bool Silent { get; set; }
        public string Text { get; set; }

        public void CreateTextResponse(
            string text,
            long chatId = 0,
            bool disableWebpagePreview = false,
            bool disableNotification = false,
            int replyToMessageId = 0
        )
        {
            ChatId = chatId;
            DisableNotification = disableNotification;
            DisableWebpagePreview = disableWebpagePreview;
            MessageType = Message.MessageType.Text;
            ReplyToMessageId = replyToMessageId;
            Text = text;
        }
    }
}