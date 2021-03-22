using System.Collections.Generic;
using System.Threading.Tasks;
using ConfigModels = Ollio.Common.Models.Config;
using TelegramBot = Telegram.Bot;
using TelegramBotTypes = Telegram.Bot.Types;

namespace Ollio.Common.Models
{
    public class Connection
    {
        public TelegramBot.ITelegramBotClient Client { get; set; }
        public string Id { get; set; }
        public Instance Instance { get; set; }
        public List<string> Plugins { get; set; } = new List<string>();
        public Task Task { get; set; }
        public string Token { get; set; }
    }
}