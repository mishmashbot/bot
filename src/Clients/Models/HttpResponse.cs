using System.IO;
using System.Net.Http;

namespace Ollio.Clients.Models
{
    public class HttpResponse
    {
        public Stream Content { get; set; }
        public bool IsSuccess { get; set; }
        public HttpResponseMessage Response { get; set; }
    }
}