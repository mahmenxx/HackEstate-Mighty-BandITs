using HackEstate.Models;
using Microsoft.AspNetCore.Mvc;

namespace HackEstate.Controllers
{
    public class PropertyController : BaseController
    {
        public IActionResult Details(int id)
        {
            var property = _propertyRepo.Get(id);
            ViewBag.AgentProperty = _agentPropertyRepo.Table.Where(m => m.PropertyId == id).FirstOrDefault();
            return View(property);
        }

        [HttpPost]
        public IActionResult SendMessage(int agentId, int propertyId, string message)
        {
            int userId = int.Parse(User.FindFirst("UserId")?.Value);

            var newChat = new ChatMessage()
            {
                FromUserId = userId,
                ToUserId = agentId,
                Message = message
            };
            _chatMessageRepo.Create(newChat);

            TempData["success"] = "Message sent successfully!";
            return RedirectToAction("Details", new { id = propertyId });
        }

    }
}
