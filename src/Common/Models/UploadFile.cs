using System;
using System.IO;
using TelegramBotInputFiles = Telegram.Bot.Types.InputFiles;

namespace Ollio.Common.Models
{
    public class UploadFile : TelegramBotInputFiles.InputOnlineFile
    {
        public UploadFile(Stream content) : base(content) { }
        public UploadFile(Uri uri) : base(uri) { }
        public UploadFile(string value) : base(value) { }
    }
}