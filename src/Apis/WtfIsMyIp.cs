using System.Threading.Tasks;
using Ollio.Apis.Http;
using Ollio.Apis.Models;
using Ollio.Apis.Models.WtfIsMyIp;

namespace Ollio.Apis
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