namespace PoolIt.Services.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IRidesService
    {
        Task<string> CreateAsync(RideServiceModel model);
        Task<IEnumerable<RideServiceModel>> GetAllUpcomingWithFreeSeatsAsync();
        Task<RideServiceModel> GetAsync(string id);
        Task<IEnumerable<RideServiceModel>> GetAllUpcomingForUserAsync(string userName);
        Task<IEnumerable<RideServiceModel>> GetAllPastForUserAsync(string userName);
        bool CanUserAccessRide(RideServiceModel rideServiceModel, string userName);
        bool IsUserOrganiser(RideServiceModel rideServiceModel, string userName);
        bool IsUserParticipant(RideServiceModel rideServiceModel, string userName);
    }
}