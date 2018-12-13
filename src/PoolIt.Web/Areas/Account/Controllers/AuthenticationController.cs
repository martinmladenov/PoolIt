namespace PoolIt.Web.Areas.Account.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models.Authentication;
    using PoolIt.Models;
    using Web.Controllers;

    [Area("Account")]
    public class AuthenticationController : BaseController
    {
        private readonly SignInManager<PoolItUser> signInManager;

        public AuthenticationController(SignInManager<PoolItUser> signInManager)
        {
            this.signInManager = signInManager;
        }

        [Route("/login")]
        public IActionResult Login(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.User.Identity.IsAuthenticated)
            {
                return this.LocalRedirect(returnUrl);
            }

            return this.View();
        }

        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> Login(UserLoginBindingModel model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.User.Identity.IsAuthenticated)
            {
                return this.LocalRedirect(returnUrl);
            }

            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            var result = await this.signInManager.PasswordSignInAsync(model.Email, model.Password,
                model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return this.LocalRedirect(returnUrl);
            }

            this.ModelState.AddModelError(string.Empty, "Invalid username or password");
            return this.View();
        }

        [Route("/register")]
        public IActionResult Register(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.User.Identity.IsAuthenticated)
            {
                return this.LocalRedirect(returnUrl);
            }

            return this.View();
        }

        [HttpPost]
        [Route("/register")]
        public async Task<IActionResult> Register(UserRegisterBindingModel model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.User.Identity.IsAuthenticated)
            {
                return this.LocalRedirect(returnUrl);
            }

            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            var user = new PoolItUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            var result = await this.signInManager.UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                this.Success(string.Format(NotificationMessages.RegistrationWelcome, model.FirstName));

                await this.signInManager.UserManager.AddToRoleAsync(user, GlobalConstants.UserRoleName);

                await this.signInManager.SignInAsync(user, isPersistent: false);
                return this.LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }

            return this.View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();

            this.Success(NotificationMessages.LoggedOut);

            return this.LocalRedirect("/");
        }

        [HttpPost]
        public IActionResult GitHubLogin(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.User.Identity.IsAuthenticated)
            {
                return this.LocalRedirect(returnUrl);
            }

            var redirectUrl = this.Url.Action("GitHubLoginCallback", "Authentication", new {returnUrl});
            var properties = this.signInManager.ConfigureExternalAuthenticationProperties("GitHub", redirectUrl);
            return new ChallengeResult("GitHub", properties);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GitHubLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.User.Identity.IsAuthenticated)
            {
                return this.LocalRedirect(returnUrl);
            }

            if (remoteError != null)
            {
                return this.RedirectToAction("Login", new {ReturnUrl = returnUrl});
            }

            var info = await this.signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return this.RedirectToAction("Login", new {ReturnUrl = returnUrl});
            }

            var result = await this.signInManager.ExternalLoginSignInAsync("GitHub", info.ProviderKey,
                isPersistent: true, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                return this.LocalRedirect(returnUrl);
            }

            this.ViewData["ReturnUrl"] = returnUrl;

            var model = new UserGitHubRegisterBindingModel();

            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                model.Email = info.Principal.FindFirstValue(ClaimTypes.Email);
            }

            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Name))
            {
                var name = info.Principal.FindFirstValue(ClaimTypes.Name);

                var split = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (split.Length >= 2)
                {
                    model.FirstName = split[0];
                    model.LastName = split[split.Length - 1];
                }
                else
                {
                    model.FirstName = name;
                }
            }

            return this.View("GitHubRegister", model);
        }

        [HttpPost]
        [Route("/register/github")]
        public async Task<IActionResult> GitHubRegister(UserGitHubRegisterBindingModel model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.User.Identity.IsAuthenticated)
            {
                return this.LocalRedirect(returnUrl);
            }

            var info = await this.signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                return this.RedirectToAction("Login", new {ReturnUrl = returnUrl});
            }

            if (this.ModelState.IsValid)
            {
                var user = new PoolItUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await this.signInManager.UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await this.signInManager.UserManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        this.Success(string.Format(NotificationMessages.RegistrationWelcome, model.FirstName));

                        await this.signInManager.UserManager.AddToRoleAsync(user, GlobalConstants.UserRoleName);

                        await this.signInManager.SignInAsync(user, isPersistent: true);
                        return this.LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            this.ViewData["ReturnUrl"] = returnUrl;
            return this.View();
        }
    }
}