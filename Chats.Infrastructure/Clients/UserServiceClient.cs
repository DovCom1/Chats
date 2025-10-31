using Chats.Core.DTOs;
using Chats.Core.Interfaces;
using System.Net.Http.Json;

namespace Chats.Infrastructure.Services
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _http;

        public UserServiceClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<UserMainDto?> GetUserMainAsync(Guid userId)
        {
            var response = await _http.GetAsync($"/api/users/{userId}/main");
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<UserMainDto>();
        }
    }
}
