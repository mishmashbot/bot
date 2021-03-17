using System;
using System.Text.Json.Serialization;

namespace Ollio.Apis.Models.Gist
{
    public class Commit
    {
        // TODO: user, change_status
        [JsonPropertyName("committed_at")]
        public DateTime CommittedAt { get; set; }
        [JsonPropertyName("version")]
        public string Version { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}