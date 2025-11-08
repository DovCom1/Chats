using Chats.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chats.Core.Models
{
    [Table("chats")]
    public class Chat
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("type")]
        public ChatType Type { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("avatar_url")]
        public string? AvatarUrl { get; set; }

        public ICollection<ChatMember> Members { get; set; } = new List<ChatMember>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
