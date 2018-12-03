namespace PoolIt.Web.Pages
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Middlewares.MiddlewareServices.Contracts;
    using Models;

    public class CaptchaModel : PageModel
    {
        private readonly IRateLimitingService rateLimitingService;

        public CaptchaModel(IRateLimitingService rateLimitingService)
        {
            this.rateLimitingService = rateLimitingService;
        }

        [BindProperty]
        public CaptchaConfirmationBindingModel Input { get; set; }

        public IActionResult OnGet(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            var clientIp = this.Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            if (clientIp == null || !this.rateLimitingService.IsClientLocked(clientIp))
            {
                return this.LocalRedirect(returnUrl);
            }

            this.Response.StatusCode = StatusCodes.Status429TooManyRequests;

            return this.Page();
        }

        public IActionResult OnPost(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            var clientIp = this.Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            if (clientIp == null || !this.rateLimitingService.IsClientLocked(clientIp))
            {
                return this.LocalRedirect(returnUrl);
            }

            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            this.rateLimitingService.UnlockClient(clientIp);

            return this.LocalRedirect(returnUrl);
        }
    }
}