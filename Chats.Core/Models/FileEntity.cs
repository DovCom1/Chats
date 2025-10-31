using System.ComponentModel.DataAnnotations.Schema;

namespace Chats.Core.Models
{
    [Table("files")]
    public class FileEntity
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("message_id")]
        public Guid MessageId { get; set; }

        [Column("file_type")]
        public string FileType { get; set; } = null!;

        [Column("url")]
        public string Url { get; set; } = null!;

        [Column("name")]
        public string Name { get; set; } = null!;

        [Column("size")]
        public long Size { get; set; }

        [Column("uploaded_at")]
        public DateTime UploadedAt { get; set; }

        public Message Message { get; set; } = null!;
    }
}
