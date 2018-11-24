namespace PoolIt.Services.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IRidesService
    {
        Task<string> Create(RideServiceModel model);
        Task<IEnumerable<RideServiceModel>> GetAllUpcomingWithFreeSeats();
        Task<RideServiceModel> Get(string id);
    }
}