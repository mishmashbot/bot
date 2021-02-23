using System.Collections.Generic;

namespace Ollio.Config.Models
{
    public class Root
    {
        public List<Client> Clients { get; set; }
        public Api Api { get; set; }
        public Owner Owner { get; set; }
    }
}