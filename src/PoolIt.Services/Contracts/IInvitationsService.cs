namespace PoolIt.Services.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IInvitationsService
    {
        Task<string> GenerateAsync(string rideId);
        Task<InvitationServiceModel> GetAsync(string key);
        Task<bool> DeleteAsync(string key);
        Task<bool> AcceptAsync(string userName, string invitationKey);
        Task<IEnumerable<InvitationServiceModel>> GetAllAsync();
    }
}