namespace Chats.Core.DTOs
{
    public class ChatHistoryDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<MessageDto>? Messages { get; set; }
    }

}
