
using System.Net.Http;

namespace Ollio.Clients.Models
{
    public class JsonResponse<T> : HttpResponse
    {
        public T Data { get; set; }
    }
}