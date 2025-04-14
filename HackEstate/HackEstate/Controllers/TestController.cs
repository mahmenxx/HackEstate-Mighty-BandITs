using Microsoft.AspNetCore.Mvc;

namespace HackEstate.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Events()
        {
            return View();
        }
        public IActionResult ViewEvent()
        {
            return View();
        }
    }
}
