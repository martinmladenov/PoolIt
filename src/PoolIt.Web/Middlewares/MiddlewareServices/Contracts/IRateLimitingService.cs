namespace PoolIt.Web.Middlewares.MiddlewareServices.Contracts
{
    public interface IRateLimitingService
    {
        bool RegisterClientRequest(string clientIp);
        bool IsClientLocked(string clientIp);
        void UnlockClient(string clientIp);
    }
}