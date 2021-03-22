using ConfigModels = Ollio.Common.Models.Config;

namespace Ollio.Common.Models
{
    public class PluginRequest
    {
        public Command Command { get; set; }
        public ConfigModels.Root Config { get; set; }
        public Message Message { get; set; }
    }
}