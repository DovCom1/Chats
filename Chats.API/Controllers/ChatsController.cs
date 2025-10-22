using Chats.Core.DTOs;
using Chats.Service.Managers;
using Microsoft.AspNetCore.Mvc;

namespace Chats.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatsController : ControllerBase
    {
        private readonly ChatsManager _chatsManager;

        public ChatsController(ChatsManager chatsManager)
        {
            _chatsManager = chatsManager;
        }

        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChat(Guid chatId)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null)
                return Unauthorized();

            if (!Guid.TryParse(userIdClaim.Value, out var currentUserId))
                return BadRequest("Invalid user id");

            var chat = await _chatsManager.GetChatAsync(chatId, currentUserId);
            if (chat == null) return NotFound();

            return Ok(chat);
        }

        [HttpGet("{chatId}/members")]
        public async Task<IActionResult> GetChatMembers(Guid chatId)
        {
            var response = await _chatsManager.GetChatMembersAsync(chatId);
            return Ok(response);
        }

        [HttpPost("{chatId}/messages")]
        public async Task<IActionResult> SendMessage(Guid chatId, [FromBody] SendMessageRequestDTO request)
        {
            await _chatsManager.SendMessageAsync(chatId, request.UserId, request.Content);
            return NoContent();
        }

        [HttpGet("{chatId}/messages")]
        public async Task<IActionResult> GetChatHistory(Guid chatId, int pageNumber = 1, int pageSize = 20)
        {
            var history = await _chatsManager.GetChatHistoryAsync(chatId, pageNumber, pageSize);
            return Ok(history);
        }

        [HttpPut("{chatId}/messages/{messageId}")]
        public async Task<IActionResult> EditMessage(Guid chatId, Guid messageId, [FromBody] SendMessageRequestDTO request)
        {
            if (!await _chatsManager.CanUserModifyMessageAsync(messageId, request.UserId))
                return Forbid();

            await _chatsManager.EditMessageAsync(chatId, messageId, request.Content, request.UserId);
            return NoContent();
        }

        [HttpDelete("{chatId}/messages/{messageId}/users/{userId}")]
        public async Task<IActionResult> DeleteMessage(Guid chatId, Guid messageId, Guid userId)
        {
            if (!await _chatsManager.CanUserModifyMessageAsync(messageId, userId))
                return Forbid();

            await _chatsManager.DeleteMessageAsync(chatId, messageId, userId);
            return NoContent();
        }

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserChats(Guid userId, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _chatsManager.GetUserChatsAsync(userId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPost("{chatId}/members")]
        public async Task<IActionResult> AddUserToChat(Guid chatId, [FromBody] ChatMemberDto request)
        {
            await _chatsManager.AddUserToChatAsync(chatId, request.UserId, request.Role);
            return NoContent();
        }

        [HttpDelete("{chatId}/members/{userId}")]
        public async Task<IActionResult> RemoveUserFromChat(Guid chatId, Guid userId)
        {
            await _chatsManager.RemoveUserFromChatAsync(chatId, userId);
            return NoContent();
        }
    }
}
