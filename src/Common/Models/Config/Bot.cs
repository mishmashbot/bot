using System.Collections.Generic;

namespace Ollio.Common.Models.Config
{
    public class Bot
    {
        public string Id { get; set; }
        public BotConfig Config { get; set; }
        public List<string> Plugins { get; set; }
        public List<Owner> Owners { get; set; }
    }
}