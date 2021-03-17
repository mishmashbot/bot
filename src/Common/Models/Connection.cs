using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TelegramBot = Telegram.Bot;

namespace Ollio.Common.Models
{
    public class Connection
    {
        public Context Context { get; set; }
        public TelegramBot.ITelegramBotClient Client { get; set; }
        public string Id { get; set; }
        public List<string> Plugins { get; set; } = new List<string>();
        public Task Task { get; set; }
        public string Token { get; set; }
    }
}