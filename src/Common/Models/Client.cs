
namespace Ollio.Models
{
    public class Client
    {
        public Services Service { get; set; }
        public string Key { get; set; }
        public string Prefix { get; set; }
        public string Server { get; set; }

        public enum Services {
            //Discord,
            //IRC,
            Telegram
        }
    }
}