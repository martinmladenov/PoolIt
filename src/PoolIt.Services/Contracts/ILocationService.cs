namespace PoolIt.Services.Contracts
{
    using System.Threading.Tasks;

    public interface ILocationService
    {
        Task<string> GetTownName(double longitude, double latitude);
    }
}