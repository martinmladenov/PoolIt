namespace PoolIt.Services.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IManufacturersService
    {
        Task<CarManufacturerServiceModel> GetByNameAsync(string name);
        Task<IEnumerable<CarManufacturerServiceModel>> GetAllAsync();
        Task<bool> CreateAsync(CarManufacturerServiceModel model);
        Task<bool> DeleteAsync(string id);
        Task<bool> UpdateAsync(CarManufacturerServiceModel model);
        Task<bool> ExistsAsync(CarManufacturerServiceModel model);
    }
}