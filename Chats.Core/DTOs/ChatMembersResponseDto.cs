namespace Chats.Core.DTOs
{
    public class ChatMembersResponseDto
    {
        public IEnumerable<ChatMemberDto> Members { get; set; } = new List<ChatMemberDto>();
    }
}
