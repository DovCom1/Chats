namespace Chats.Core.DTOs
{
    public class ChatListResponseDto
    {
        public IEnumerable<ChatMainDto> Chats { get; set; } = new List<ChatMainDto>();
    }
}
