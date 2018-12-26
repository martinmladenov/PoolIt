namespace PoolIt.Services.Contracts
{
    using System.Threading.Tasks;

    public interface IPersonalDataService
    {
        Task<string> GetPersonalDataForUserJson(string userId);
    }
}