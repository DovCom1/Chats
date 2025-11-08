using System.ComponentModel.DataAnnotations.Schema;

namespace Chats.Core.Models
{
    [Table("reactions")]
    public class Reaction
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("message_id")]
        public Guid MessageId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("reaction_type")]
        public string ReactionType { get; set; } = null!;

        public Message Message { get; set; } = null!;
    }
}
