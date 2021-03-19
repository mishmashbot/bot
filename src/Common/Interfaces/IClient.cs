using System.Collections.Generic;
using System.Threading.Tasks;
using Ollio.Common.Enums;
using Ollio.Common.Models;

namespace Ollio.Common.Interfaces
{
    public interface IClient
    {
        ClientType Type { get; }

        Task Connect();
        Task Disconnect();
        Task<User> GetMe();
        Task SetCommands(IDictionary<string, string> commands);
        Task<Message> SendPhotoMessage(PluginResponse response);
        Task<Message> SendTextMessage(PluginResponse response);
        Task<bool> TestConnection();
        Task ToggleBusy(PluginRequest request);
    }
}