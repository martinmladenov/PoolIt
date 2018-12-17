namespace PoolIt.Services.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IModelsService
    {
        Task<bool> CreateAsync(CarModelServiceModel serviceModel);
        Task<IEnumerable<CarModelServiceModel>> GetAllByManufacturerAsync(string manufacturerId);
        Task<bool> ExistsAsync(CarModelServiceModel serviceModel);
        Task<bool> DeleteAsync(string id);
        Task<bool> UpdateAsync(CarModelServiceModel model);
    }
}