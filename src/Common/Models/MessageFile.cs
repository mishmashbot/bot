using System;
using System.IO;

namespace Ollio.Models
{
    public class MessageFile : Telegram.Bot.Types.InputFiles.InputOnlineFile
    {
        public MessageFile(Stream content) : base(content) { }
        public MessageFile(Uri uri) : base(uri) { }
        public MessageFile(string value) : base(value) { }
    }
}