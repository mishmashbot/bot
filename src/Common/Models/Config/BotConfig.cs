
namespace Ollio.Common.Models.Config
{
    public class BotConfig
    {
        public Api Api { get; set; }
        public bool NoOllio { get; set; } = false;
        public char Prefix { get; set; } = '/';
        public string Server { get; set; } = "";
        public int Tick { get; set; } = 10;
        public string Token { get; set; }
    }
}