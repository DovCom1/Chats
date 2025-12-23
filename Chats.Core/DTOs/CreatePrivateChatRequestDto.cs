using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Core.DTOs
{
    public class CreatePrivateChatRequestDto
    {
        public List<Guid> UserIds { get; set; } = new();
    }
}
