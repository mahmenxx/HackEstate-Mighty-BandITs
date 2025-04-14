using HackEstate.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HackEstate.Controllers
{
    public class ChatController : BaseController
    {
        [HttpGet]
        public IActionResult GetRecentMessages()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var messagesList = _chatMessageRepo.Table.Where(m => m.FromUserId == userId || m.ToUserId == userId).ToList();
            var messages = messagesList
                .OrderByDescending(m => m.ToUserId)
                .Take(20)
                .Select(m => new
                {
                    sender = m.FromUser.FirstName,
                    text = m.Message,
                    isFromMe = m.FromUser.Id == userId
                }).ToList();

            return Json(messages);
        }

        [HttpGet]
        public IActionResult GetMessagesWithUser(int userId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var messages = _chatMessageRepo.Table
                .Where(m => (m.FromUserId == currentUserId && m.ToUserId == userId) ||
                            (m.FromUserId == userId && m.ToUserId == currentUserId))
                .Select(m => new {
                    sender = m.FromUser.FirstName,
                    text = m.Message,
                    isFromMe = m.FromUserId == currentUserId
                }).ToList();

            return Json(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage()
        {
            var fromUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //var message = new ChatMessage
            //{
            //    FromUserId = fromUserId,
            //    ToUserId = dto.ReceiverId,
            //    Message = dto.Message,
            //    Timestamp = DateTime.Now
            //};

            //_chatMessageRepo.Create(message);

            return Ok();
        }

        public class ChatMessageDto
        {
            public int ReceiverId { get; set; }
            public string Message { get; set; }
        }
    }
}
