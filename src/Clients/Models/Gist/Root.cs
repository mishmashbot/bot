using System;
using System.Text.Json.Serialization;

namespace Ollio.Clients.Models.Gist
{
    public class Root
    {
        [JsonPropertyName("comments")]
        public int Comments { get; set; }
        [JsonPropertyName("comments_url")]
        public string CommentsUrl { get; set; }
        [JsonPropertyName("commits_url")]
        public string CommitsUrl { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("forks_url")]
        public string ForksUrl { get; set; }
        [JsonPropertyName("git_pull_url")]
        public string GitPullUrl { get; set; }
        [JsonPropertyName("git_push_url")]
        public string GitPushUrl { get; set; }
        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("node_id")]
        public string NodeId { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}