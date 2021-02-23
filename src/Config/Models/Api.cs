using YamlDotNet.Serialization;

namespace Ollio.Config.Models
{
    public class Api
    {
        [YamlMember(Alias = "duckduckgo", ApplyNamingConventions = false)]
        public ApiDuckDuckGo DuckDuckGo { get; set; }
        [YamlMember(Alias = "openweathermap", ApplyNamingConventions = false)]
        public ApiOpenWeatherMap OpenWeatherMap { get; set; }
        [YamlMember(Alias = "wolfram", ApplyNamingConventions = false)]
        public ApiWolframAlpha WolframAlpha { get; set; }
    }
}