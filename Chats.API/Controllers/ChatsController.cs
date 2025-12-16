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
        private readonly MessagesManager _messagesManager;

        public ChatsController(ChatsManager chatsManager, MessagesManager messagesManager)
        {
            _chatsManager = chatsManager;
            _messagesManager = messagesManager;
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
            if (chat == null)
                return NotFound();

            return Ok(chat);
        }

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserChats(Guid userId, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _chatsManager.GetUserChatsAsync(userId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{chatId}/members")]
        public async Task<IActionResult> GetChatMembers(Guid chatId)
        {
            var response = await _chatsManager.GetChatMembersAsync(chatId);
            return Ok(response);
        }

        [HttpPost("{chatId}/members")]
        public async Task<IActionResult> AddUserToChat(Guid chatId, [FromBody] ChatMemberDto request)
        {
            if (request.UserId == Guid.Empty)
                return BadRequest("UserId is required");

            await _chatsManager.AddUserToChatAsync(chatId, request.UserId, request.Role, request.Nickname);
            return NoContent();
        }

        [HttpDelete("{chatId}/members/{userId}")]
        public async Task<IActionResult> RemoveUserFromChat(Guid chatId, Guid userId)
        {
            await _chatsManager.RemoveUserFromChatAsync(chatId, userId);
            return NoContent();
        }

        [HttpPost("add")]
        public async Task<IActionResult> CreatePrivateChat([FromBody] CreatePrivateChatRequestDto request)
        {
            if (request == null || request.UserIds.Count != 2)
                return BadRequest("Exactly two userIds are required");

            var chat = await _chatsManager.CreatePrivateChatAsync(
                request.UserIds[0],
                request.UserIds[1]);

            return Ok(chat);
        }

        [HttpPost("{chatId}/messages")]
        public async Task<IActionResult> SendMessage(Guid chatId, [FromBody] SendMessageRequestDTO request)
        {
            if (request == null)
                return BadRequest("Request body is missing.");
            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest("Content cannot be empty.");
            if (request.UserId == Guid.Empty)
                return BadRequest("UserId is required.");

            var message = await _messagesManager.SendMessageAsync(chatId, request);
            return NoContent();
        }

        [HttpPost("messages")]
        public async Task<IActionResult> SendNewMessage([FromBody] SendMessageRequestDTO request)
        {
            if (request == null)
                return BadRequest("Request body is missing.");
            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest("Content cannot be empty.");
            if (request.UserId == Guid.Empty)
                return BadRequest("UserId is required.");
            if (request.ReceiverId == null)
                return BadRequest("ReceiverId is required for a new chat.");

            var message = await _messagesManager.SendMessageAsync(null, request);
            return NoContent();
        }

        [HttpGet("{chatId}/messages")]
        public async Task<IActionResult> GetChatHistory(Guid chatId, int pageNumber = 1, int pageSize = 20)
        {
            var history = await _messagesManager.GetChatHistoryAsync(chatId, pageNumber, pageSize);
            return Ok(history);
        }

        [HttpPut("{chatId}/messages/{messageId}")]
        public async Task<IActionResult> EditMessage(Guid chatId, Guid messageId, [FromBody] SendMessageRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest("Content cannot be empty.");

            if (!await _messagesManager.CanUserModifyMessageAsync(messageId, request.UserId))
                return Forbid();

            await _messagesManager.EditMessageAsync(chatId, messageId, request.Content, request.UserId);
            return NoContent();
        }

        [HttpDelete("{chatId}/messages/{messageId}/users/{userId}")]
        public async Task<IActionResult> DeleteMessage(Guid chatId, Guid messageId, Guid userId)
        {
            if (!await _messagesManager.CanUserModifyMessageAsync(messageId, userId))
                return Forbid();

            await _messagesManager.DeleteMessageAsync(chatId, messageId, userId);
            return NoContent();
        }
    }
}
