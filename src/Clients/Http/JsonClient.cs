using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ollio.Clients.Http
{
    public class JsonClient<T>
    {
        public static HttpClient Client { get; set; }

        public async Task<T> Get(string url, IDictionary<string, IEnumerable<string>> headers = null)
        {
            return await Call(new Uri(url), HttpMethod.Get, headers);
        }

        public async Task<T> Call(
            Uri uri, HttpMethod method,
            IDictionary<string, IEnumerable<string>> headers = null
        )
        {
            if(Client == null)
                Client = new HttpClient();

            var request = new HttpRequestMessage()
            {
                Method = method,
                RequestUri = uri
            };

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            var response = await Client.SendAsync(request);
            var content = await response.Content.ReadAsStreamAsync();
            var deserialized = await JsonSerializer.DeserializeAsync<T>(
                content,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            );

            return deserialized;
        }
    }
}
