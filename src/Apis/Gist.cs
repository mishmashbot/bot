using System.Collections.Generic;
using System.Threading.Tasks;
using Ollio.Apis.Http;
using Ollio.Apis.Models;
using Ollio.Apis.Models.Gist;

namespace Ollio.Apis
{
    public class Gist
    {
        const string BaseUrl = "https://api.github.com/gists";

        public async Task<JsonResponse<Root>> Get(string gistId)
        {
            var client = new JsonClient<Root>();
            var request = await client.Get($"{BaseUrl}/{gistId}");
            return request;
        }

        public async Task<JsonResponse<List<Commit>>> GetCommits(string gistId, int perPage = 0)
        {
            var client = new JsonClient<List<Commit>>();
            var request = await client.Get(
                $"{BaseUrl}/{gistId}/commits?per_page={perPage}",
                queries: new Dictionary<string, string> {
                    { "per_page", (perPage > 0) ? perPage.ToString() : "" }
                }
            );
            return request;
        }

        public async Task<HttpResponse> GetRaw(string username, string gistId, string versionId, string filename)
        {
            var client = new HttpsClient();
            var request = await client.Get($"https://gist.githubusercontent.com/{username}/{gistId}/raw/{versionId}/{filename}");
            return request;
        }
    }
}