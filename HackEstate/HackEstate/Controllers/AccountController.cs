﻿using HackEstate.Authentication;
using HackEstate.Models;
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
        public async Task<IActionResult> Login(string email, string password)
        {
            var validLogin = _userRepo.Table.Where(m => m.Email == email && m.Password.Equals(password)).FirstOrDefault();

            if (validLogin != null)
            {
                if(validLogin.RoleId == 5)
                {
                    await this._signInManager.SignInAsync(validLogin);
                    return RedirectToAction("Dashboard", "Admin");
                }
                if (validLogin.Status.Equals("INACTIVE") && validLogin.RoleId != 1)
                {
                    await this._signInManager.SignInAsync(validLogin);
                    return RedirectToAction("IntroQuiz", "Home");
                }
                if (validLogin.IdentificationCardUrl != null && validLogin.RoleId == 1)
                {
                    await this._signInManager.SignInAsync(validLogin);
                    return RedirectToAction("VerifyAgent", "Home");
                }
                await this._signInManager.SignInAsync(validLogin);
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                TempData["error"] = "Invalid login attempt";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string FirstName, string LastName, string Email, string ContactNo, string Username, string Password, string Address, int Role)
        {
            var newUser = new User()
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Password = Password,
                Address = Address,
                Username = Username,
                RoleId = Role,
                Status = "INACTIVE"
            };

            _userRepo.Create(newUser);

            TempData["success"] = "Successfully registered. You may now login.";
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await this._signInManager.SignOutAsync();
            HttpContext.Session.Remove("IsFaceVerified");
            return RedirectToAction("Login", "Account");
        }
    }
}
