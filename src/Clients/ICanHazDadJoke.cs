using System.Threading.Tasks;
using Ollio.Clients.Http;
using Ollio.Clients.Models;
using Ollio.Clients.Models.ICanHazDadJoke;

namespace Ollio.Clients
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