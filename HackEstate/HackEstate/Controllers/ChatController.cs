using Microsoft.AspNetCore.Mvc;
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

    }
}
