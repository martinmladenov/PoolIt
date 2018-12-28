namespace PoolIt.Services.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IContactMessagesService
    {
        Task<bool> CreateAsync(ContactMessageServiceModel model);
        Task<IEnumerable<ContactMessageServiceModel>> GetAllAsync();
        Task<bool> DeleteAsync(string id);
    }
}