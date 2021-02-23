using System.Collections.Generic;

namespace Ollio.Config.Models
{
    public class ClientConfig
    {
        public List<string> Channels { get; set; }
        public string Key { get; set; }
        public string Prefix { get; set; }
        public string Server { get; set; }
    }
}