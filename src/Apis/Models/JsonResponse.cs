
using System.Net.Http;

namespace Ollio.Apis.Models
{
    public class JsonResponse<T> : HttpResponse
    {
        public T Data { get; set; }
    }
}