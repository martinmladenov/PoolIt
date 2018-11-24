namespace PoolIt.Services.Contracts
{
    using System.Threading.Tasks;

    public interface ILocationService
    {
        Task<string> GetTownNameAsync(double longitude, double latitude);
    }
}