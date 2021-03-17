using System.Text.Json.Serialization;

namespace Ollio.Apis.Models.WtfIsMyIp
{
    public class Root
    {
        [JsonPropertyName("YourFuckingCountryCode")]
        public string CountryCode { get; set; }
        [JsonPropertyName("YourFuckingHostname")]
        public string Hostname { get; set; }
        [JsonPropertyName("YourFuckingIPAddress")]
        public string IPAddress { get; set; }
        [JsonPropertyName("YourFuckingISP")]
        public string ISP { get; set; }
        [JsonPropertyName("YourFuckingLocation")]
        public string Location { get; set; }
        [JsonPropertyName("YourFuckingTorExit")]
        public bool TorExit { get; set; }
    }
}