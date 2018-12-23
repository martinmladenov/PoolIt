namespace PoolIt.Services.Contracts
{
    using System.Threading.Tasks;

    public interface ILocationHelper
    {
        Task<string> GetTownNameAsync(double longitude, double latitude);
    }
}