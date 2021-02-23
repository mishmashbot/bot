
namespace Ollio.Models
{
    public class Connection<T>
    {
        public T Client { get; set; } = default(T);
        public Client Config { get; set; }
    }
}