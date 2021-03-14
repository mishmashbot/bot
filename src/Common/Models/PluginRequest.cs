
namespace Ollio.Common.Models
{
    public class PluginRequest
    {
        public Command Command { get; set; }
        public Message Message { get; set; }
        public RuntimeInfo Runtime { get; set; }
    }
}