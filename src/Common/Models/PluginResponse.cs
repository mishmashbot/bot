
namespace Ollio.Common.Models
{
    public class PluginResponse
    {
        public Message Message { get; set; }
        public string RawOutput { get; set; }
        public bool Silent { get; set; }
    }
}