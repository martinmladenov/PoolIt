namespace PoolIt.Services.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IJoinRequestsService
    {
        Task<bool> CreateAsync(JoinRequestServiceModel model);
        Task<IEnumerable<JoinRequestServiceModel>> GetReceivedForUserAsync(string userName);
        Task<JoinRequestServiceModel> GetAsync(string id);
        Task<bool> AcceptAsync(string id);
        Task<bool> DeleteAsync(string id);
        bool CanUserSendJoinRequest(RideServiceModel rideServiceModel, string userName);
        Task<bool> CanUserAccessRequestAsync(string id, string userName);
    }
}