using HackEstate.Managers;
using HackEstate.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace HackEstate.Controllers
{
    public class PropertyController : BaseController
    {

        private readonly MailManager _mailManager;
        public PropertyController(MailManager mailManager)
        {
            _mailManager = mailManager;
        }
        public async Task<IActionResult> Details(int id)
        {
            var property = _propertyRepo.Get(id);
            ViewBag.AgentProperty = _agentPropertyRepo.Table.Where(m => m.PropertyId == id).FirstOrDefault();
            var chatMessages = _chatMessageRepo.Table.Where(m => m.PropertyId == id).ToList();
            ViewBag.ChatMessages = chatMessages;
            ViewBag.IsSold = true;
            
            ViewBag.EcoScore = await EcoScore(property);
            return View(property);
        }

        [HttpPost]
        public async Task<int> EcoScore(Property property)
        {
            int ecoScore = 0;
            try
            {

                var propertyDTO = new
                {
                    property.Description,
                    property.Location
                };


                string answerJson = JsonConvert.SerializeObject(propertyDTO);

                string input = $"Property Description: {answerJson}";

                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
            {
                { "input", input }
            };

                    var content = new FormUrlEncodedContent(values);

                    // Post the request to your Gemini model API endpoint
                    var response = await client.PostAsync("https://divine-booking-456823-t4.et.r.appspot.com/ecoScore", content);
                    response.EnsureSuccessStatusCode();

                    // Read the API response
                    var responseString = await response.Content.ReadAsStringAsync();
                    var jResponse = JObject.Parse(responseString);


                    string recommendationRaw = jResponse["recommendation"]?.ToString();

                    var recommendation = jResponse["recommendation"].ToString();
                    ecoScore = int.Parse(recommendation);
                }

                // Return the list of recommended agents
                return ecoScore;
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return 0; // Return an empty list if an error occurs
            }
        }


        public IActionResult Edit(int id)
        {
            var property = _propertyRepo.Get(id);

            return View(property);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(
            int id,
            string? title,
            string? location,
            double? price,
            int? lotSize,
            int? bedroomQty,
            int? bathroomQty,
            string? propertyType,
            string? status,
            string? amenities,
            string? description,
            List<IFormFile>? images
            )
        {
            var property = _propertyRepo.Get(id);
            if (property == null) return NotFound();

            property.Title = title;
            property.Location = location;
            property.Price = price;
            property.LotSize = lotSize;
            property.BedroomQty = bedroomQty;
            property.BathroomQty = bathroomQty;
            property.PropertyType = propertyType;
            property.Status = status;
            property.Amenities = amenities;
            property.Description = description;

            _propertyRepo.Update(property.Id, property);

            // Save uploaded images
            if (images != null && images.Any())
            {
                var relativeFolder = "Attachments/PropertyImages";
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativeFolder);
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                foreach (var image in images)
                {
                    if (image.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                        var fullPath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        var propertyImage = new PropertyImage
                        {
                            PropertyId = property.Id,
                            ImageUrl = Path.Combine(relativeFolder, fileName).Replace("\\", "/") // Save as "Attachments/PropertyImages/xyz.jpg"
                        };

                        _propertyImageRepo.Create(propertyImage); // Save image record
                    }
                }
            }
            TempData["success"] = "Updated successfully!";
            return RedirectToAction("Details", new { id = property.Id });
        }


        [HttpPost]
        public IActionResult SendMessage(int toUserId, int propertyId, string message)
        {
            int userId = int.Parse(User.FindFirst("UserId")?.Value);

            var newChat = new ChatMessage()
            {
                FromUserId = userId,
                ToUserId = toUserId,
                Message = message,
                PropertyId = propertyId
            };
            _chatMessageRepo.Create(newChat);

            var sender = _userRepo.Get(userId);
            var toUser = _userRepo.Get(toUserId);

            TempData["success"] = "Message sent successfully!";

            string errResponse = "";
            _mailManager.SendMessageToEmail(toUser.Email, "Message Notification", toUser.FirstName, sender.Role.RoleName, sender.FirstName, message, ref errResponse);

            return RedirectToAction("Details", new { id = propertyId });
        }

    }
}
