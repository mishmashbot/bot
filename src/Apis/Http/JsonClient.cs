using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Ollio.Apis.Models;

namespace Ollio.Apis.Http
{
    public class JsonClient<T>
    {
        public async Task<JsonResponse<T>> Get(
            string url, 
            IDictionary<string, string> queries = null,
            IDictionary<string, IEnumerable<string>> headers = null
        )
        {
            return await Call(url, HttpMethod.Get, queries, headers);
        }

        public async Task<JsonResponse<T>> Call(
            string url, HttpMethod method,
            IDictionary<string, string> queries = null,
            IDictionary<string, IEnumerable<string>> headers = null
        )
        {
            var httpClient = new HttpsClient();
            var result = await httpClient.Call(url, method, queries, headers);

            var deserialized = await JsonSerializer.DeserializeAsync<T>(
                result.Content,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            );

            return new JsonResponse<T>
            {
                Content = result.Content,
                Data = deserialized,
                IsSuccess = result.IsSuccess,
                Response = result.Response
            };
        }
    }
}
