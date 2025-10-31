using Chats.Core.DTOs;

namespace Chats.Core.Interfaces
{
    public interface IUserServiceClient
    {
        Task<UserMainDto?> GetUserMainAsync(Guid userId);
    }
}
