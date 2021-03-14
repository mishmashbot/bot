using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Ollio.Config.Models;

namespace Ollio.Config
{
    public class ConfigLoader
    {
        const string DefaultConfig = @"";

        // TODO: Error on non-unique IDs
        public static bool UpdateConfig(string path)
        {
            StringReader input;

            if(!File.Exists(path)) {
                // TODO: Create default config
                return false;
            }

            using (var reader = new StreamReader(path)) {
                input = new StringReader(reader.ReadToEnd());
            }

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var config = deserializer.Deserialize<Root>(input);
            ConfigState.Current = config;

            return true;
        }
    }
}