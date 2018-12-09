namespace PoolIt.Services.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface ICarsService
    {
        Task<bool> CreateAsync(CarServiceModel model);
        Task<IEnumerable<CarServiceModel>> GetAllForUserAsync(string userName);
        Task<CarServiceModel> GetAsync(string id);
        bool IsUserOwner(CarServiceModel carServiceModel, string userName);
        Task<bool> UpdateAsync(CarServiceModel model);
    }
}