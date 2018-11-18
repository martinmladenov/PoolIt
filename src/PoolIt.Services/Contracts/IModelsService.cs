namespace PoolIt.Services.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IModelsService
    {
        Task<CarModelServiceModel> Get(string id);
        Task<bool> Create(CarModelServiceModel serviceModel);
        Task<IEnumerable<CarModelServiceModel>> GetAllByManufacturer(string manufacturerId);
    }
}