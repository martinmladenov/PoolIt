namespace PoolIt.Services.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface ICarsService
    {
        Task<bool> Create(CarServiceModel model);
        Task<IEnumerable<CarServiceModel>> GetAllForUser(string userName);
        Task<CarServiceModel> Get(string id);
    }
}