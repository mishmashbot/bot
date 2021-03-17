using System.Threading.Tasks;
using Ollio.Apis.Http;
using Ollio.Apis.Models;
using Ollio.Apis.Models.ICanHazDadJoke;

namespace Ollio.Apis
{
    public class ICanHazDadJoke
    {
        const string BaseUrl = "https://icanhazdadjoke.com";

        public async Task<JsonResponse<Root>> Get()
        {
            var client = new JsonClient<Root>();
            var request = await client.Get($"{BaseUrl}");
            return request;
        }
    }
}