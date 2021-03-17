using YamlDotNet.Serialization;
using Ollio.Common.Models.Config.Apis;

namespace Ollio.Common.Models.Config
{
    public class Api
    {
        [YamlMember(Alias = "duckduckgo", ApplyNamingConventions = false)]
        public DuckDuckGo DuckDuckGo { get; set; }
        [YamlMember(Alias = "openweathermap", ApplyNamingConventions = false)]
        public OpenWeatherMap OpenWeatherMap { get; set; }
        [YamlMember(Alias = "wolfram", ApplyNamingConventions = false)]
        public WolframAlpha WolframAlpha { get; set; }
    }
}