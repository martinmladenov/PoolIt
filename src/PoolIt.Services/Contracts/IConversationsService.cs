namespace PoolIt.Services.Contracts
{
    using System.Threading.Tasks;
    using Models;

    public interface IConversationsService
    {
        Task<ConversationServiceModel> GetAsync(string id);
        Task<MessageServiceModel> SendMessageAsync(MessageServiceModel model);
    }
}