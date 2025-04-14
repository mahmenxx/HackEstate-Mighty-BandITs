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

        private readonly SignInManager _signInManager;
        private readonly TokenValidationParametersFactory _tokenValidationParametersFactory;
        private readonly TokenProviderOptionsFactory _tokenProviderOptionsFactory;
        private readonly IConfiguration _appConfiguration;

        public AdminController(SignInManager signInManager,
                            IHttpContextAccessor httpContextAccessor,
                            ILoggerFactory loggerFactory,
                            IConfiguration configuration,
                            TokenValidationParametersFactory tokenValidationParametersFactory,
                            TokenProviderOptionsFactory tokenProviderOptionsFactory
            )
        {
            _signInManager = signInManager;
            this._tokenProviderOptionsFactory = tokenProviderOptionsFactory;
            this._tokenValidationParametersFactory = tokenValidationParametersFactory;
            this._appConfiguration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Dashboard()
        {
            ViewData["title"] = "Admin Dashboard";
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Properties()
        {
            ViewData["title"] = "Admin Properties";
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Users()
        {
            ViewData["title"] = "Admin Users";
            return View();
        }
    }
}
