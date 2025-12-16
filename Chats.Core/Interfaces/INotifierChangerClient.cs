using Chats.Core.DTOs;

namespace Chats.Core.Interfaces
{
    public interface INotifierChangerClient
    {
        Task SendMessageEventAsync(MessageEventDto eventDto);
    }
}
