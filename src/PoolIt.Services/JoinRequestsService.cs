namespace PoolIt.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
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

        public async Task<IEnumerable<JoinRequestServiceModel>> GetReceivedForUser(string userName)
        {
            if (userName == null)
            {
                return null;
            }

            var requests = await this.context.JoinRequests
                .Where(r => r.Ride.Car.Owner.UserName == userName)
                .ProjectTo<JoinRequestServiceModel>()
                .ToArrayAsync();

            return requests;
        }

        public async Task<JoinRequestServiceModel> Get(string id)
        {
            if (id == null)
            {
                return null;
            }

            var request = await this.context.JoinRequests
                .ProjectTo<JoinRequestServiceModel>()
                .SingleOrDefaultAsync(r => r.Id == id);

            return request;
        }

        public async Task<bool> Accept(string id)
        {
            if (id == null)
            {
                return false;
            }

            var request = await this.context.JoinRequests
                .SingleOrDefaultAsync(r => r.Id == id);

            var userRide = new UserRide
            {
                UserId = request.UserId,
                RideId = request.RideId,
            };

            await this.context.UserRides.AddAsync(userRide);

            this.context.JoinRequests.Remove(request);

            await this.context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Delete(string id)
        {
            if (id == null)
            {
                return false;
            }

            var request = await this.context.JoinRequests
                .SingleOrDefaultAsync(r => r.Id == id);

            this.context.JoinRequests.Remove(request);

            await this.context.SaveChangesAsync();

            return true;
        }
    }
}