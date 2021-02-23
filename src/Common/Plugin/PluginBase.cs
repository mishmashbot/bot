
namespace Ollio.Plugin
{
    public interface PluginBase
    {
        string Id { get; }
        string Name { get; }
        int Version { get; }
        string[] Commands { get; }
    
        PluginResponse Invoke(PluginRequest request);
        void Startup() { }
    }
}