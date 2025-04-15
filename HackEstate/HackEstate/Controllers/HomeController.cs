using HackEstate.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;


namespace HackEstate.Controllers
{
    public class HomeController : BaseController
    {
        [Authorize]
        public IActionResult IntroQuiz()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SubmitIntroQuiz(
             // Buyer fields
             string Buyer_PropertyType,
             string Buyer_Location,
             int? Buyer_BudgetMin,
             int? Buyer_BudgetMax,
             string Buyer_Timeline,

             // Seller fields
             string Seller_PropertyType,
             string Seller_Location,
             int? Seller_Price,
             string Seller_Timeline,
             string Seller_Help,

             // Common fields
             string CommMethod,
             string AgentPreference
         )
        {
            string userId = User.FindFirst("UserId")?.Value;
            var recommendedAgents = new List<User>();

            var user = _userRepo.Get(int.Parse(userId));
            if (user.RoleId == 2)
            {
                var newAnswer = new UserQuizAnswer()
                {
                    UserId = user.Id,
                    BudgetMax = Buyer_BudgetMax,
                    BudgetMin = Buyer_BudgetMin,
                    WhenToBuy = Buyer_Timeline,
                    MostImportantInAgent = AgentPreference,
                    PreferCommunication = CommMethod,
                    TypeOfProperty = Buyer_PropertyType,
                    PreferredLocation = Buyer_Location
                };
                _userQuizAnswerRepo.Create(newAnswer);
                recommendedAgents = await AgentsRecommendedByAi(newAnswer, user);
            }
            else if (user.RoleId == 3)
            {
                var newAnswer = new UserQuizAnswer()
                {
                    UserId = user.Id,
                    WhenToBuy = Seller_Timeline,
                    TypeOfProperty = Seller_PropertyType,
                    MostImportantInAgent = AgentPreference,
                    PreferredLocation = Seller_Location,
                    PreferCommunication = CommMethod
                };
                _userQuizAnswerRepo.Create(newAnswer);
                recommendedAgents = await AgentsRecommendedByAi(newAnswer, user);
            }
            user.Status = "ACTIVE";
            _userRepo.Update(user.Id, user);
            TempData["success"] = "Answers recorded. You will be recommended of agents via AI smart-matchmaking!";
            ViewBag.Agents = recommendedAgents;
            return RedirectToAction("Dashboard");
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            if (User.Identity.IsAuthenticated)
            {
                int userId = int.Parse(User.FindFirst("UserId")?.Value);

                var user = _userRepo.Get(userId);

                var userAnswer = _userQuizAnswerRepo.Table.Where(m => m.UserId == userId).FirstOrDefault();
                if (user.RoleId == 1)
                {
                    string isVerified = HttpContext.Session.GetString("IsFaceVerified");

                    if (string.IsNullOrEmpty(isVerified) || isVerified != "true")
                    {
                        return RedirectToAction("VerifyAgent", "Home");
                    }
                }
                if (userAnswer == null)
                {
                }
                else
                {
                    ViewBag.Agents = await AgentsRecommendedByAi(userAnswer, user);
                }

                var messages = _chatMessageRepo.Table
                    .Where(m => m.FromUserId == userId || m.ToUserId == userId)
                    .ToList();

                var chattedUsers = messages
                    .Select(m => m.FromUserId == userId ? m.ToUser : m.FromUser)
                    .DistinctBy(u => u.Id) // Requires System.Linq for .DistinctBy
                    .ToList();

                ViewBag.ChattedUsers = chattedUsers;
                return View();
            }
            return RedirectToAction("Login", "Account");
        }

        [Authorize]
        public IActionResult Properties()
        {
            int userId = int.Parse(User.FindFirst("UserId")?.Value);

            var user = _userRepo.Get(userId);

            if (user.RoleId == 3)
            {
                var yourProperties = _agentPropertyRepo.Table.Where(m => m.Property.UserId == userId).ToList();

                ViewBag.YourProperties = yourProperties;
            }
            if (user.RoleId == 1)
            {
                var yourProperties = _agentPropertyRepo.Table.Where(m => m.AgentId == userId).ToList();

                ViewBag.YourProperties = yourProperties;
            }

            return View();
        }



        [Authorize]
        [HttpPost]
        public async Task<List<User>> AgentsRecommendedByAi(UserQuizAnswer answer, User user)
        {
            try
            {
                List<User> recommendedAgents = new List<User>();
                var answerDTO = new
                {
                    answer.TypeOfProperty,
                    answer.PreferredLocation,
                    answer.BudgetMin,
                    answer.BudgetMax,
                    answer.WhenToBuy,
                    answer.PreferCommunication,
                    answer.MostImportantInAgent
                };

                var userDTO = new
                {
                    user.RoleId,
                    user.Address,
                    user.Contact,
                    user.Status,
                    user.Description
                };

                var agentsDTO = _userRepo.Table
                    .Where(m => m.RoleId == 1)
                    .Select(agent => new
                    {
                        agent.Id,
                        agent.Address,
                        agent.Description
                    })
                    .ToList();

                string answerJson = JsonConvert.SerializeObject(answerDTO);
                string userJson = JsonConvert.SerializeObject(userDTO);
                var agents = _userRepo.Table.Where(m => m.RoleId == 1).ToList();
                string agentsJson = JsonConvert.SerializeObject(agentsDTO);

                string input = $"User's Answers: {answerJson},\nUser: {userJson},\nList of Agents: {agentsJson}";

                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
            {
                { "input", input }
            };

                    var content = new FormUrlEncodedContent(values);

                    // Post the request to your Gemini model API endpoint
                    var response = await client.PostAsync("https://divine-booking-456823-t4.et.r.appspot.com/assignAgent", content);
                    response.EnsureSuccessStatusCode();

                    // Read the API response
                    var responseString = await response.Content.ReadAsStringAsync();
                    var jResponse = JObject.Parse(responseString);


                    string recommendationRaw = jResponse["recommendation"]?.ToString();

                    if (!string.IsNullOrWhiteSpace(recommendationRaw))
                    {
                        // Split by new lines, parse each line as int
                        var agentIds = recommendationRaw
                            .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(idStr => int.TryParse(idStr.Trim(), out var id) ? id : 0)
                            .Where(id => id > 0)
                            .ToList();

                        // Optional: Fetch full User data from DB if you have a service or db context
                        foreach (var agentId in agentIds)
                        {
                            // Example: If you fetch from a database, use your service/repo here
                            var agent = _userRepo.Get(agentId);
                            if (agent != null)
                                recommendedAgents.Add(agent);
                        }
                    }
                }

                // Return the list of recommended agents
                return recommendedAgents;
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return new List<User>(); // Return an empty list if an error occurs
            }
        }

        

        [Authorize]
        [HttpPost]
        public IActionResult AddProperty(
            string? Title,
            string? Description,
            string? PropertyType,
            int? BathroomQty,
            int? BedroomQty,
            int? LotSize,
            double? Price,
            string? Location,
            string? Amenities)
        {
            if (User.IsInRole("3")) // Only allow sellers
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var property = new Property
                {
                    Title = Title,
                    Description = Description,
                    PropertyType = PropertyType,
                    BathroomQty = BathroomQty,
                    BedroomQty = BedroomQty,
                    LotSize = LotSize,
                    Price = Price,
                    Location = Location,
                    Amenities = Amenities,
                    UserId = userId,
                    Status = "AVAILABLE"
                };

                _propertyRepo.Create(property);

                TempData["success"] = "Property added successfully!";
                return RedirectToAction("Dashboard"); // or another view
            }
            else
            {
                TempData["error"] = "Invalid access. Only sellers can add properties.";
                return RedirectToAction("Dashboard");
            }
        }

        [Authorize]
        public IActionResult Agents()
        {
            var agents = _userRepo.Table.Where(m => m.RoleId == 1).ToList();
            return View(agents);
        }

        public IActionResult Events()
        {
            return View();
        }

        public IActionResult VerifyAgent()
        {
            return View();
        }

        [Authorize]
        public IActionResult Profile()
        {
            int userId = int.Parse(User.FindFirst("UserId")?.Value);

            var user = _userRepo.Get(userId);
//            var users = new List<User>
//{
//    new User { Username = "agent_john", Email = "john.agent@example.com", RoleId = 1, FirstName = "John", LastName = "Doe", Address = "123 Makati Ave", Contact = "09171234567", Password = "Password123!", Status = "Active" },
//    new User { Username = "buyer_anna", Email = "anna.buyer@example.com", RoleId = 2, FirstName = "Anna", LastName = "Reyes", Address = "456 Taguig St", Contact = "09181234567", Password = "Password123!", Status = "Active" },
//    new User { Username = "seller_mike", Email = "mike.seller@example.com", RoleId = 3, FirstName = "Mike", LastName = "Santos", Address = "789 Pasig Blvd", Contact = "09191234567", Password = "Password123!", Status = "Active" },
//    new User { Username = "agent_rose", Email = "rose.agent@example.com", RoleId = 1, FirstName = "Rose", LastName = "Cruz", Address = "Greenhills, San Juan", Contact = "09201234567", Password = "Password123!", Status = "Active" },
//    new User { Username = "buyer_luis", Email = "luis.buyer@example.com", RoleId = 2, FirstName = "Luis", LastName = "Lopez", Address = "QC, Commonwealth", Contact = "09211234567", Password = "Password123!", Status = "Active" },
//    new User { Username = "seller_emma", Email = "emma.seller@example.com", RoleId = 3, FirstName = "Emma", LastName = "Dela Cruz", Address = "Ortigas Center", Contact = "09221234567", Password = "Password123!", Status = "Active" },
//    new User { Username = "agent_kyle", Email = "kyle.agent@example.com", RoleId = 1, FirstName = "Kyle", LastName = "Garcia", Address = "BGC, Taguig", Contact = "09231234567", Password = "Password123!", Status = "Active" },
//    new User { Username = "buyer_ivy", Email = "ivy.buyer@example.com", RoleId = 2, FirstName = "Ivy", LastName = "Morales", Address = "Alabang, Muntinlupa", Contact = "09241234567", Password = "Password123!", Status = "Active" },
//    new User { Username = "seller_nico", Email = "nico.seller@example.com", RoleId = 3, FirstName = "Nico", LastName = "Fernandez", Address = "Las Piñas", Contact = "09251234567", Password = "Password123!", Status = "Active" },
//    new User { Username = "agent_bea", Email = "bea.agent@example.com", RoleId = 1, FirstName = "Bea", LastName = "Navarro", Address = "Marikina", Contact = "09261234567", Password = "Password123!", Status = "Active" }
//};

//            // Insert all to the database
//            foreach (var user1 in users)
//            {
//                _userRepo.Create(user1);
//            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User model, IFormFile IdentificationCardFile)
        {
            if (ModelState.IsValid)
            {
                var user = _userRepo.Get(model.Id);

                if (user == null)
                    return NotFound();

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Username = model.Username;
                user.Email = model.Email;
                user.Contact = model.Contact;
                user.Address = model.Address;
                user.Description = model.Description;

                // Handle ID image upload
                if (IdentificationCardFile != null && IdentificationCardFile.Length > 0)
                {
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Attachments", "IdentificationCards");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    var fileName = Guid.NewGuid() + Path.GetExtension(IdentificationCardFile.FileName);
                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await IdentificationCardFile.CopyToAsync(stream);
                    }

                    // Save the relative path (e.g., Attachments/IdentificationCards/filename.jpg)
                    user.IdentificationCardUrl = Path.Combine("Attachments", "IdentificationCards", fileName).Replace("\\", "/");
                }

                _userRepo.Update(user.Id, user);
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }

            return View("Profile", model);
        }


        [AllowAnonymous]
        public IActionResult Landing()
        {
            return View();
        }
    }
}
