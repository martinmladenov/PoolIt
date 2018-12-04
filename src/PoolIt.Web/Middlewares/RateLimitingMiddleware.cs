namespace PoolIt.Web.Middlewares
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.Extensions.Logging;
    using MiddlewareServices.Contracts;

    public class RateLimitingMiddleware
    {
        private const string CaptchaPageLocalUrlWithReturnUrl = "/captcha?returnUrl=";

        private static readonly string[] ExcludedUrls =
        {
            "/captcha",
            "/account/authentication/logout"
        };

        private readonly RequestDelegate next;

        private readonly IRateLimitingService service;

        private readonly ILogger<RateLimitingMiddleware> logger;

        public RateLimitingMiddleware(RequestDelegate next, IRateLimitingService service,
            ILogger<RateLimitingMiddleware> logger)
        {
            this.next = next;
            this.service = service;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var url = context.Request.GetEncodedPathAndQuery().Split('?')[0].ToLower();
            var isExcluded = ExcludedUrls.Contains(url);

            if (isExcluded)
            {
                await this.next(context);
                return;
            }

            var clientIp = context.Connection?.RemoteIpAddress?.ToString();

            if (clientIp == null)
            {
                this.logger.LogWarning("Could not get client IP!");
                await this.next(context);
                return;
            }

            var isLocked = this.service.RegisterClientRequest(clientIp);

            if (isLocked)
            {
                this.logger.LogWarning(
                    $"User {context.User?.Identity?.Name} with IP {clientIp} was redirected to captcha page");
                context.Response.Redirect(
                    CaptchaPageLocalUrlWithReturnUrl + context.Request.GetEncodedPathAndQuery());
                return;
            }

            await this.next(context);
        }
    }
}