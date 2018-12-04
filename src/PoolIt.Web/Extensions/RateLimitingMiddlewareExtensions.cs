namespace PoolIt.Web.Extensions
{
    using Microsoft.AspNetCore.Builder;
    using Middlewares;

    public static class RateLimitingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimitingMiddleware>();
        }
    }
}