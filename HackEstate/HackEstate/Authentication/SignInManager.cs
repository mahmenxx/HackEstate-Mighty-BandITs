using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using HackEstate.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using HackEstate.Configuration;

namespace HackEstate.Authentication
{
    /// <summary>
    /// SignInManager
    /// </summary>
    public class SignInManager
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        //public LoginUser user { get; set; }

        /// <summary>
        /// Initializes a new instance of the SignInManager class.
        /// </summary>
        public SignInManager()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SignInManager class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="accountService">The account service.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public SignInManager(IConfiguration configuration,
                             IHttpContextAccessor httpContextAccessor)
        {
            this._configuration = configuration;
            this._httpContextAccessor = httpContextAccessor;
            //user = new LoginUser();
        }

        /// <summary>
        /// Gets the claims identity.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The successfully completed task</returns>
        public Task<ClaimsIdentity> GetClaimsIdentity(string username, string password)
        {
            ClaimsIdentity claimsIdentity = null;
            User userData = new User();

            //user.loginResult = LoginResult.Success;//TODO this._accountService.AuthenticateUser(username, password, ref userData);

            //if (user.loginResult == LoginResult.Failed)
            //{
            //    return Task.FromResult<ClaimsIdentity>(null);
            //}

            //user.userData = userData;
            claimsIdentity = CreateClaimsIdentity(userData);
            return Task.FromResult(claimsIdentity);
        }

        /// <summary>
        /// Creates the claims identity.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Instance of ClaimsIdentity</returns>
        public ClaimsIdentity CreateClaimsIdentity(User user)
        {
            var token = _configuration.GetTokenAuthentication();
            var userId = user.Id.ToString();
            var name = user.FirstName;

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId, ClaimValueTypes.String, "hackestate"),
                new Claim(ClaimTypes.Name, name, ClaimValueTypes.String, "hackestate"),
                new Claim(ClaimTypes.Role, user.RoleId.ToString(), ClaimValueTypes.Integer, "hackestate"),

                new Claim("UserId", userId, ClaimValueTypes.String, "hackestate"),
                new Claim("UserName", name, ClaimValueTypes.String, "hackestate")

            };
            return new ClaimsIdentity(claims, "hackestate");
        }

        /// <summary>
        /// Creates the claims principal.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>Created claims principal</returns>
        public IPrincipal CreateClaimsPrincipal(ClaimsIdentity identity)
        {
            var identities = new List<ClaimsIdentity>();
            identities.Add(identity);
            return this.CreateClaimsPrincipal(identities);
        }

        /// <summary>
        /// Creates the claims principal.
        /// </summary>
        /// <param name="identities">The identities.</param>
        /// <returns>Created claims principal</returns>
        public IPrincipal CreateClaimsPrincipal(IEnumerable<ClaimsIdentity> identities)
        {
            var principal = new ClaimsPrincipal(identities);
            return principal;
        }

        /// <summary>
        /// Signs in user asynchronously
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="isPersistent">if set to <c>true</c> [is persistent].</param>
        public async Task SignInAsync(User userDetails, bool isPersistent = false)
        {
            var claimsIdentity = this.CreateClaimsIdentity(userDetails);
            var principal = this.CreateClaimsPrincipal(claimsIdentity);
            await this.SignInAsync(principal, isPersistent);
        }

        /// <summary>
        /// Signs in user asynchronously
        /// </summary>
        /// <param name="principal">The principal.</param>
        /// <param name="isPersistent">if set to <c>true</c> [is persistent].</param>
        public async Task SignInAsync(IPrincipal principal, bool isPersistent = false)
        {
            var token = _configuration.GetTokenAuthentication();
            await _httpContextAccessor
                .HttpContext
                .SignInAsync(
                            "hackestate",
                            (ClaimsPrincipal)principal,
                            new AuthenticationProperties
                            {
                                ExpiresUtc = DateTime.UtcNow.AddMinutes(token.ExpirationMinutes),
                                IsPersistent = isPersistent,
                                AllowRefresh = false
                            });
        }

        /// <summary>
        /// Signs out user asynchronously
        /// </summary>
        public async Task SignOutAsync()
        {
            var token = _configuration.GetTokenAuthentication();
            await _httpContextAccessor.HttpContext.SignOutAsync("hackestate");
        }
    }
}
