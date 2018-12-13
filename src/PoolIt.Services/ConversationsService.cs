namespace PoolIt.Services
{
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data.Common;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using PoolIt.Models;

    public class ConversationsService : BaseService, IConversationsService
    {
        private readonly IRepository<Conversation> conversationsRepository;
        private readonly IRepository<PoolItUser> usersRepository;
        private readonly IRepository<Message> messagesRepository;

        public ConversationsService(IRepository<Conversation> conversationsRepository,
            IRepository<PoolItUser> usersRepository, IRepository<Message> messagesRepository)
        {
            this.conversationsRepository = conversationsRepository;
            this.usersRepository = usersRepository;
            this.messagesRepository = messagesRepository;
        }

        public async Task<ConversationServiceModel> GetAsync(string id)
        {
            if (id == null)
            {
                return null;
            }

            var conversation = await this.conversationsRepository.All()
                .ProjectTo<ConversationServiceModel>()
                .SingleOrDefaultAsync(c => c.Id == id);

            return conversation;
        }

        public async Task<MessageServiceModel> SendMessageAsync(MessageServiceModel model)
        {
            if (!this.IsEntityStateValid(model))
            {
                return null;
            }

            var author = await this.usersRepository.All()
                .SingleOrDefaultAsync(u => u.UserName == model.Author.UserName);

            var conversationFound = await this.conversationsRepository.All()
                .AnyAsync(c => c.Id == model.ConversationId);

            if (author == null || !conversationFound)
            {
                return null;
            }

            var message = Mapper.Map<Message>(model);

            message.Author = author;

            await this.messagesRepository.AddAsync(message);

            await this.messagesRepository.SaveChangesAsync();

            return Mapper.Map<MessageServiceModel>(message);
        }
    }
}