using System.Collections.Generic;

namespace Ollio.Config.Models
{
    public class Context
    {
        public string Id { get; set; }
        public ContextConfig Config { get; set; }
        public ContextBrand Brand { get; set; }
        public List<string> Plugins { get; set; }
    }
}