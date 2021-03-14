using System;
using System.Collections.Generic;
using System.Threading;
using Telegram.Bot;

namespace Ollio.Common.Models
{
    public class Connection
    {
        public string BrandName { get; set; }
        public ITelegramBotClient Client { get; set; }
        public DateTime DateStarted { get; set; }
        public string Name { get; set; }
        public User Me { get; set; }
        public User Owner { get; set; }
        public List<string> Plugins { get; set; } = new List<string>();
        public string Prefix { get; set; }
        public Thread Thread { get; set; }
        public string Token { get; set; }
    }
}