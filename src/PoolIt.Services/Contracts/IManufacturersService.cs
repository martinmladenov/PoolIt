namespace PoolIt.Services.Contracts
{
    using System.Threading.Tasks;
    using Models;

    public interface IManufacturersService
    {
        Task<CarManufacturerServiceModel> Get(string name);
        Task<bool> Create(CarManufacturerServiceModel model);
    }
}