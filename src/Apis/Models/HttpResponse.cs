using System.IO;
using System.Net.Http;

namespace Ollio.Apis.Models
{
    public class HttpResponse
    {
        public Stream Content { get; set; }
        public HttpResponseMessage Response { get; set; }
    }
}