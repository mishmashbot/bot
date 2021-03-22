using ConfigModels = Ollio.Common.Models.Config;
using TelegramBotTypes = Telegram.Bot.Types;

namespace Ollio.Common.Models
{
    public class PluginRequest
    {
        public Command Command { get; set; }
        public ConfigModels.Root Config { get; set; }
        public TelegramBotTypes.User Me { get; set; }
        public Message Message { get; set; }
        public RuntimeInfo RuntimeInfo { get; set; }
    }
}