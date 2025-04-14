using HackEstate.Authentication;
using HackEstate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlTypes;
using System.Security.Claims;

namespace HackEstate.Controllers
{
    public class AdminController : BaseController
    {
        private readonly IConfiguration _appConfiguration;

        public AdminController(
                            IHttpContextAccessor httpContextAccessor,
                            ILoggerFactory loggerFactory,
                            IConfiguration configuration
            )
        {
            this._appConfiguration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Dashboard()
        {
            ViewData["title"] = "Dashboard";
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Properties()
        {
            ViewData["title"] = "Properties";
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Users()
        {
            ViewData["title"] = "Users";
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Events()
        {
            ViewData["title"] = "Events";
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Settings()
        {
            ViewData["title"] = "Settings";
            return View();
        }
    }
}
