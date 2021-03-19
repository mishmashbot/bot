using System;
using System.Threading.Tasks;

namespace Ollio.Plugin
{
    public interface IPlugin
    {
        string Id { get; }
        string Name { get; }
        Subscription Subscription { 
            get => new Subscription();
            set => new Subscription();
        }
        int Version { get; }
    
        void OnInit() { }
        void OnConnect() { }
        void OnConnect(Connection connection) { }
        Task OnConnectAsync() { return null; }
        Task OnConnectAsync(Connection connection) { return null; }
        // OnDisconnect?
        Response OnTick(Request request) { return null; }
        Task<Response> OnTickAsync(Request request) { return null; }
        Response OnMessage(Request request) { return null; }
        Task<Response> OnMessageAsync(Request request) { return null; }
    }
}