using Ollio.Common.Enums;

namespace Ollio.Common.Models.Config
{
    public class Client
    {
        public char Prefix { get; set; } = '/';
        public string Server { get; set; } = "";
        public string Token { get; set; }
    }
}