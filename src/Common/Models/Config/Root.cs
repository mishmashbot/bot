using System.Collections.Generic;

namespace Ollio.Common.Models.Config
{
    public class Root
    {
        public List<Bot> Bots { get; set; }
        public Api Api { get; set; }
    }
}