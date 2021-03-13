using System.Collections.Generic;

namespace Ollio.Models
{
    public class PluginSubscription
    {
        public List<string> Callbacks { get; set; } = new List<string>();
        public List<string> Commands { get; set; } = new List<string>();
        public bool OnAudio { get; set; }
        public bool OnCallback { get; set; } = true;
        public bool OnCommand { get; set; } = true;
        public bool OnDocument { get; set; }
        public bool OnPhoto { get; set; }
        public bool OnSticker { get; set; }
        public bool OnText { get; set; }
        public bool OnVideo { get; set; }
    }
}