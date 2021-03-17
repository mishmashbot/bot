using System.Threading.Tasks;
using Ollio.Clients.Http;
using Ollio.Clients.Models;
using Ollio.Clients.Models.WtfIsMyIp;

namespace Ollio.Clients
{
    public class WtfIsMyIp
    {
        const string BaseUrl = "https://wtfismyip.com/json";

        public async Task<JsonResponse<Root>> Get()
        {
            var client = new JsonClient<Root>();
            var request = await client.Get($"{BaseUrl}");
            return request;
        }
    }
}