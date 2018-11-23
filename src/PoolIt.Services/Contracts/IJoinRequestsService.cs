namespace PoolIt.Services.Contracts
{
    using System.Threading.Tasks;
    using Models;

    public interface IJoinRequestsService
    {
        Task<bool> Create(JoinRequestServiceModel model);
    }
}