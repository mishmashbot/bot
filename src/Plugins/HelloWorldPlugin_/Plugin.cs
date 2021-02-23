using System;
using Mishmash.Plugin;

namespace HelloWorldPlugin
{
    public class Program : PluginBase
    {
        public string[] Commands
        {
            get => new string[] {
                "hello", "hello2"
            };
        }
        public string Name { get => "Hello World!"; }
        public Guid Serial { get => new Guid("b22046ef-2a46-4030-ab2a-ebfc671684dd"); }
        public int Version { get => 1; }

        public PluginResponse Invoke(PluginRequest request)
        {
            PluginResponse response = new PluginResponse();

            response.RawOutput = "Yo!";

            return response;
        }
    }
}