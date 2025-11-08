using System.ComponentModel.DataAnnotations.Schema;

namespace Chats.Core.Models
{
    [Table("chat_members")]
    public class ChatMember
    {
        [Column("chat_id")]
        public Guid ChatId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("role")]
        public string Role { get; set; } = "member";

        [Column("nickname")]
        public string? Nickname { get; set; }

        public Chat Chat { get; set; } = null!;
    }
}
