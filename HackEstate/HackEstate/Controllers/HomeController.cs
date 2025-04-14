using HackEstate.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HackEstate.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult IntroQuiz()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
