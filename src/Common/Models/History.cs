using System;

namespace Ollio.Models
{
    public class History
    {
        public DateTime DateCreated { get; set; }
        public int MessageInId { get; set; }
        public int MessageOutId { get; set; }
        public string PluginId { get; set; }
    }
}