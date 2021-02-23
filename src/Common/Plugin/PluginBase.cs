using System;

namespace Mishmash.Plugin
{
    public interface PluginBase
    {
        string[] Commands { get; }
        string Name { get; }
        Guid Serial { get; }
        int Version { get; }
    
        PluginResponse Invoke(PluginRequest request);
    }
}