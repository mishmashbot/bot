using System;
using System.Collections.Generic;
using ConfigModels = Ollio.Common.Models.Config;

namespace Ollio.Common.Models
{
    public class Context
    {
        public ConfigModels.BotConfig Config { get; set; }
        public DateTime DateConnected { get; set; }
        public DateTime DateStarted { get; set; }
        public User Me { get; set; }
        public List<User> Owners { get; set; }
        public Random Random { get; set; }
        public RuntimeInfo RuntimeInfo { get; set; }
    }
}