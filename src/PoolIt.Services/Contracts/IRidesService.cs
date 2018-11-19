namespace PoolIt.Services.Contracts
{
    using System.Threading.Tasks;
    using Models;

    public interface IRidesService
    {
        Task<string> Create(RideServiceModel model);
    }
}