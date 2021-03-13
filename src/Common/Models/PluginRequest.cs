
namespace Ollio.Models
{
    public class PluginRequest
    {
        public string Command { get; set; }
        public Message Message { get; set; }
        public RuntimeInfo Runtime { get; set; }
    }
}