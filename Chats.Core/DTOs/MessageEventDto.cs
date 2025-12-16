namespace Chats.Core.DTOs
{
    public class MessageEventDto
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public Guid ChatId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? SenderName { get; set; }
        public string? ReceiverName { get; set; }
        public string? ChatName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
