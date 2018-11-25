namespace PoolIt.Web.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using PoolIt.Models;

    public class AuthenticationController : Controller
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
        [Route("/logout")]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await this.signInManager.SignOutAsync();
            
            if (returnUrl != null)
            {
                return this.LocalRedirect(returnUrl);
            }

            return this.RedirectToRoute("/");
        }
    }
}