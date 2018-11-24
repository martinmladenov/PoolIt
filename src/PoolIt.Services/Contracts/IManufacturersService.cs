namespace PoolIt.Services.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IManufacturersService
    {
        Task<CarManufacturerServiceModel> GetAsync(string name);
        Task<IEnumerable<CarManufacturerServiceModel>> GetAllAsync();
    }
}