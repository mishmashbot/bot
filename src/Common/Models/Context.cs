using System;
using System.Collections.Generic;
using ConfigModels = Ollio.Common.Models.Config;
using TelegramBotTypes = Telegram.Bot.Types;

namespace Ollio.Common.Models
{
    public class Context
    {
        public ConfigModels.BotConfig Config { get; set; }
        public DateTime DateConnected { get; set; }
        public DateTime DateStarted { get; set; }
        public TelegramBotTypes.User Me { get; set; }
        public List<TelegramBotTypes.User> Owners { get; set; }
        public Random Random { get; set; }
        public RuntimeInfo RuntimeInfo { get; set; }
    }
}