using System.Threading.Tasks;

namespace Ollio.Plugin
{
    public interface IPlugin
    {
        string Id { get; }
        string Name { get; }
        Subscription Subscription { get => new Subscription(); }
        int Version { get; }
    
        void OnInit() { }
        void OnInit(Connection connection) { }
        Task OnInitAsync() { return null; }
        Task OnInitAsync(Connection connection) { return null; }
        Response OnMessage(Request request) { return null; }
        Task<Response> OnMessageAsync(Request request) { return null; }
    }
}