using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Ollio.Apis.Models;

namespace Ollio.Apis.Http
{
    public class HttpsClient
    {
        public static HttpClient Client { get; set; }

        public async Task<HttpResponse> Get(
            string url, 
            IDictionary<string, string> queries = null,
            IDictionary<string, IEnumerable<string>> headers = null
        )
        {
            return await Call(url, HttpMethod.Get, queries, headers);
        }

        public async Task<HttpResponse> Call(
            string url, HttpMethod method,
            IDictionary<string, string> queries = null,
            IDictionary<string, IEnumerable<string>> headers = null
        )
        {
            if(Client == null)
            {
                Client = new HttpClient();
                // TODO: Build the UAS progrmatically
                Client.DefaultRequestHeaders.Add("User-Agent", $"Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Ollio/{Assembly.GetExecutingAssembly().GetName().Version}");
            }

            if(queries != null)
            {
                url += "?";

                foreach(var query in queries)
                    if(!String.IsNullOrEmpty(query.Value))
                        url += $"{query.Key}={query.Value}&";
            }

            var request = new HttpRequestMessage()
            {
                Method = method,
                RequestUri = new Uri(url)
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

            return new HttpResponse
            {
                Content = content,
                IsSuccess = response.IsSuccessStatusCode,
                Response = response
            };
        }
    }
}
