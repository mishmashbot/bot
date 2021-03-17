using System.Collections.Generic;

namespace Ollio.Common.Models
{
    public class PluginSubscription
    {
        public List<string> Callbacks { get; set; } = new List<string>();
        public Dictionary<string, string> Commands { get; set; } = new Dictionary<string, string>();
        public bool OnAudio { get; set; }
        public bool OnDocument { get; set; }
        public bool OnPhoto { get; set; }
        public bool OnSticker { get; set; }
        public bool OnText { get; set; }
        public bool OnVideo { get; set; }
    }
}