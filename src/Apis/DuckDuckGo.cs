using System.Threading.Tasks;
using Ollio.Apis.Http;
using Ollio.Apis.Models;
using Ollio.Apis.Models.DuckDuckGo;

namespace Ollio.Apis
{
    // TODO: Work out why this isn't working
    public class DuckDuckGo
    {
        const string BaseUrl = "https://api.duckduckgo.com";
        string Key { get; set; } = "Ollio";

        public DuckDuckGo(string t)
        {
            Key = t;
        }

        public async Task<JsonResponse<Root>> Get(string query)
        {
            var client = new JsonClient<Root>();
            var request = await client.Get($"{BaseUrl}/?q={query}&format=json&t={Key}");
            return request;
        }
    }
}