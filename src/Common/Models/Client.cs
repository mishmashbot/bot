using System;

namespace Ollio.Models
{
    public class Client
    {
        public Guid Id { get; set; }
        public dynamic Data { get; set; }
        public Services Service { get; set; }

        public enum Services {
            Telegram
        }
    }
}