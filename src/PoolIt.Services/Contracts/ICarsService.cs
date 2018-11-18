namespace PoolIt.Services.Contracts
{
    using System.Threading.Tasks;
    using Models;

    public interface ICarsService
    {
        Task<bool> Create(CarServiceModel model);
    }
}