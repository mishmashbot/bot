using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Ollio.Config.Models;
using Ollio.Utilities;

namespace Ollio.Config
{
    public class ConfigLoader
    {
        // TODO: Error on non-unique IDs
        public static void UpdateConfig(string[] arguments = null)
        {
            StringReader input;

            using (var reader = new StreamReader(
                Path.Combine(RuntimeUtilities.GetConfigRoot(), "config.yaml")
            )) {
                input = new StringReader(reader.ReadToEnd());
            }

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var config = deserializer.Deserialize<Root>(input);
            ConfigState.Current = config;
        }

        const string DefaultConfig = @"";
    }
}