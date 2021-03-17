using System.Threading.Tasks;

namespace Ollio.Plugin
{
    public interface IPlugin
    {
        string Id { get; }
        string Name { get; }
        Subscription Subscription { get; }
        int Version { get; }
    
        Response OnMessage(Request request) { return null; }
        Task<Response> OnMessageAsync(Request request) { return null; }
        void OnInit() { }
        Response OnTick(Request request) { return null; }
    }
}