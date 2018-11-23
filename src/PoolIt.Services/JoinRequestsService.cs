namespace PoolIt.Services
{
    using System.Threading.Tasks;
    using AutoMapper;
    using Contracts;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using PoolIt.Models;

    public class JoinRequestsService : DataService, IJoinRequestsService
    {
        public JoinRequestsService(PoolItDbContext context) : base(context)
        {
        }
        
        public async Task<bool> Create(JoinRequestServiceModel model)
        {
            if (!this.IsEntityStateValid(model))
            {
                return false;
            }

            if (!await this.context.Rides
                .AnyAsync(r => r.Id == model.RideId))
            {
                return false;
            }

            var user = await this.context.Users
                .FirstOrDefaultAsync(u => u.UserName == model.User.UserName);

            if (user == null)
            {
                return false;
            }

            var joinRequest = Mapper.Map<JoinRequest>(model);

            joinRequest.User = user;

            await this.context.JoinRequests.AddAsync(joinRequest);

            await this.context.SaveChangesAsync();

            return true;
        }
    }
}