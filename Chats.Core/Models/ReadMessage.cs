using System.ComponentModel.DataAnnotations.Schema;

namespace Chats.Core.Models
{
    [Table("read_messages")]
    public class ReadMessage
    {
        [Column("message_id")]
        public Guid MessageId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("read_at")]
        public DateTime ReadAt { get; set; }

        public Message Message { get; set; } = null!;
    }
}
