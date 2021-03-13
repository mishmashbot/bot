using System.Collections.Generic;

namespace Ollio.Models
{
    // TODO: Buttons
    //       Thumbs
    public class Message
    {
        public int Duration { get; set; }
        public List<MessageFile> Files { get; set; }
        public int Height { get; set; }
        public MessageOptions Options { get; set; }
        public string Performer { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
        public MessageType Type { get; set; } = MessageType.Text;
        public int Width { get; set; }

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
        }
    }
}