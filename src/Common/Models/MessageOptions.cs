
namespace Ollio.Models
{
    public class MessageOptions
    {
        bool DisableNotification { get; set; } = false;
        bool DisableWebPreview { get; set; } = false;
        bool SupportsStreaming { get; set; } = true;
    }
}