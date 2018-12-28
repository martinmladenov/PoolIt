namespace PoolIt.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data.Common;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using PoolIt.Models;

    public class ContactMessagesService : BaseService, IContactMessagesService
    {
        private readonly IRepository<ContactMessage> contactMessagesRepository;
        private readonly IRepository<PoolItUser> usersRepository;

        public ContactMessagesService(IRepository<ContactMessage> contactMessagesRepository,
            IRepository<PoolItUser> usersRepository)
        {
            this.contactMessagesRepository = contactMessagesRepository;
            this.usersRepository = usersRepository;
        }

        public async Task<bool> CreateAsync(ContactMessageServiceModel model)
        {
            if (!this.IsEntityStateValid(model))
            {
                return false;
            }

            if (model.UserId != null)
            {
                if (!await this.usersRepository.All().AnyAsync(u => u.Id == model.UserId))
                {
                    return false;
                }

                model.FullName = null;
                model.Email = null;
            }

            var message = Mapper.Map<ContactMessage>(model);

            await this.contactMessagesRepository.AddAsync(message);

            await this.contactMessagesRepository.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ContactMessageServiceModel>> GetAllAsync()
        {
            var messages = await this.contactMessagesRepository.All()
                .ProjectTo<ContactMessageServiceModel>()
                .ToArrayAsync();

            return messages;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (id == null)
            {
                return false;
            }

            var message = await this.contactMessagesRepository.All()
                .SingleOrDefaultAsync(r => r.Id == id);

            if (message == null)
            {
                return false;
            }

            this.contactMessagesRepository.Remove(message);

            await this.contactMessagesRepository.SaveChangesAsync();

            return true;
        }
    }
}