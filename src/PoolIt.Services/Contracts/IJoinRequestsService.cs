namespace PoolIt.Services.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IJoinRequestsService
    {
        Task<bool> Create(JoinRequestServiceModel model);
        Task<IEnumerable<JoinRequestServiceModel>> GetReceivedForUser(string userName);
        Task<JoinRequestServiceModel> Get(string id);
        Task<bool> Accept(string id);
        Task<bool> Delete(string id);
    }
}