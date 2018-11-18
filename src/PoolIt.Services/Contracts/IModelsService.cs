namespace PoolIt.Services.Contracts
{
    using System.Threading.Tasks;
    using Models;

    public interface IModelsService
    {
        Task<CarModelServiceModel> Get(string id);
        Task<bool> Create(CarModelServiceModel serviceModel);
    }
}