using System;
using System.Collections.Generic;
using System.Threading;
using Telegram.Bot;

namespace Ollio.Models
{
    public class Connection
    {
        public string BrandName { get; set; }
        public ITelegramBotClient Client { get; set; }
        public DateTime DateStarted { get; set; }
        public string Name { get; set; }
        public User Owner { get; set; }
        public List<string> Plugins { get; set; } = new List<string>();
        public Thread Thread { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
    }
}