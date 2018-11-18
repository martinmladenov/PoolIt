namespace PoolIt.Web.Areas.Identity.Pages.Account
{
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using PoolIt.Models;

    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<PoolItUser> signInManager;

        private readonly UserManager<PoolItUser> userManager;
//        private readonly ILogger<RegisterModel> logger;
//        private readonly IEmailSender emailSender;

        public RegisterModel(
            UserManager<PoolItUser> userManager,
            SignInManager<PoolItUser> signInManager)
//            ILogger<RegisterModel> logger,
//            IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
//            this.logger = logger;
//            this.emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [StringLength(30, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 2)]
            [Display(Name = "First name")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(30, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 2)]
            [Display(Name = "Last name")]
            public string LastName { get; set; }
        }

        public IActionResult OnGet(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.User.Identity.IsAuthenticated)
            {
                return this.LocalRedirect(returnUrl);
            }

            this.ReturnUrl = returnUrl;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.User.Identity.IsAuthenticated)
            {
                return this.LocalRedirect(returnUrl);
            }

            if (this.ModelState.IsValid)
            {
                var user = new PoolItUser
                {
                    UserName = this.Input.Email,
                    Email = this.Input.Email,
                    FirstName = this.Input.FirstName,
                    LastName = this.Input.LastName
                };
                var result = await this.userManager.CreateAsync(user, this.Input.Password);
                if (result.Succeeded)
                {
//                    this.logger.LogInformation("User created a new account with password.");
//
//                    var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
//                    var callbackUrl = this.Url.Page(
//                        "/Account/ConfirmEmail",
//                        pageHandler: null,
//                        values: new { userId = user.Id, code = code },
//                        protocol: this.Request.Scheme);
//
//                    await this.emailSender.SendEmailAsync(this.Input.Email, "Confirm your email",
//                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await this.signInManager.SignInAsync(user, isPersistent: false);
                    return this.LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return this.Page();
        }
    }
}