using System.Collections.Generic;

namespace Ollio.Config.Models
{
    public class Bot
    {
        public string Id { get; set; }
        public BotConfig Config { get; set; }
        public BotBrand Brand { get; set; }
        public List<string> Plugins { get; set; }
    }
}