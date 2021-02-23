using System.Collections.Generic;

namespace Ollio.Config.Models
{
    public class Client
    {
        public string Type { get; set; }
        public ClientConfig Config { get; set; }
        public ClientBrand Brand { get; set; }
        public List<string> Plugins { get; set; }
    }
}