using Telegram.Bot;
using Ollio.Models.Requests;
using Ollio.Models.Responses;

namespace Ollio.Models
{
    public interface PluginBase
    {
        string Id { get; }
        string Name { get; }
        int Version { get; }
        string[] Commands { get; }
    
        PluginResponse Invoke(PluginRequest request);
        void Startup() { }
        void Startup(Connection<TelegramBotClient> telegram) { }
    }
}