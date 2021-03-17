
namespace Ollio.Common.Models
{
    public class PluginRequest
    {
        public Command Command { get; set; }
        public Context Context { get; set; }
        public Message Message { get; set; }
    }
}