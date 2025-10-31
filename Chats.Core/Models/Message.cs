using System.ComponentModel.DataAnnotations.Schema;

namespace Chats.Core.Models
{
    [Table("messages")]
    public class Message
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("chat_id")]
        public Guid ChatId { get; set; }

        [Column("sender_id")]
        public Guid SenderId { get; set; }

        [Column("content")]
        public string Content { get; set; } = null!;

        [Column("sent_at")]
        public DateTime SentAt { get; set; }

        [Column("edited_at")]
        public DateTime? EditedAt { get; set; }

        [Column("deleted")]
        public bool Deleted { get; set; }

        public Chat Chat { get; set; } = null!;
        public ICollection<FileEntity> Files { get; set; } = new List<FileEntity>();
        public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
        public ICollection<ReadMessage> ReadByUsers { get; set; } = new List<ReadMessage>();
    }
}
