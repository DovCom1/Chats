using Chats.Core.DTOs;
using Chats.Core.Interfaces;
using System.Net.Http.Json;


namespace Chats.Infrastructure.Clients
{
    public class NotifierChangerClient : INotifierChangerClient
    {
        private readonly HttpClient _httpClient;

        public NotifierChangerClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendMessageEventAsync(MessageEventDto eventDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/internal/Event/message", eventDto);
            response.EnsureSuccessStatusCode();
        }
    }
}
