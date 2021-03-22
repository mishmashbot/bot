using ConfigModels = Ollio.Common.Models.Config;
using TelegramBotTypes = Telegram.Bot.Types;

namespace Ollio.Common.Models
{
    public class Instance
    {
        public ConfigModels.Bot Config { get; set; }
        public TelegramBotTypes.User Me { get; set; }
        public RuntimeInfo RuntimeInfo { get; set; }
    }
}