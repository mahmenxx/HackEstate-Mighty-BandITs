using HackEstate.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlTypes;
using System.Security.Claims;

namespace HackEstate.Controllers
{
    public class AccountController : BaseController
    {

        private readonly SignInManager _signInManager;
        private readonly TokenValidationParametersFactory _tokenValidationParametersFactory;
        private readonly TokenProviderOptionsFactory _tokenProviderOptionsFactory;
        private readonly IConfiguration _appConfiguration;

        public AccountController(SignInManager signInManager,
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
        public IActionResult Login()
        {
            ViewData["title"] = "EvenTahan Login";
            if (User.Identity.IsAuthenticated)
            {
                var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                // Redirect based on role
                switch (role)
                {
                    case "1":
                        return RedirectToAction("Dashboard", "Home");
                    case "2":
                        return RedirectToAction("Dashboard", "Home");
                    case "3":
                        return RedirectToAction("Dashboard", "Home");
                    case "4":
                        return RedirectToAction("Dashboard", "Home");
                    case "5":
                        return RedirectToAction("AdminDashboard", "Admin");
                    default:
                        return RedirectToAction("Error", "Shared");
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var validLogin = _userRepo.Table.Where(m => m.Username == username && m.Password.Equals(password)).FirstOrDefault();

            if (validLogin != null)
            {
                await this._signInManager.SignInAsync(validLogin);
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                TempData["error"] = "Invalid login attempt";
                return View();
            }
        }

    }
}
