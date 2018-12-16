namespace PoolIt.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data.Common;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using PoolIt.Models;

    public class JoinRequestsService : BaseService, IJoinRequestsService
    {
        private readonly IRepository<JoinRequest> joinRequestsRepository;
        private readonly IRepository<Ride> ridesRepository;
        private readonly IRepository<PoolItUser> usersRepository;
        private readonly IRepository<UserRide> userRidesRepository;

        public JoinRequestsService(IRepository<JoinRequest> joinRequestsRepository, IRepository<Ride> ridesRepository,
            IRepository<PoolItUser> usersRepository, IRepository<UserRide> userRidesRepository)
        {
            this.joinRequestsRepository = joinRequestsRepository;
            this.ridesRepository = ridesRepository;
            this.usersRepository = usersRepository;
            this.userRidesRepository = userRidesRepository;
        }

        public async Task<bool> CreateAsync(JoinRequestServiceModel model)
        {
            if (!this.IsEntityStateValid(model))
            {
                return false;
            }

            if (!await this.ridesRepository.All()
                .AnyAsync(r => r.Id == model.RideId))
            {
                return false;
            }

            var user = await this.usersRepository.All()
                .SingleOrDefaultAsync(u => u.UserName == model.User.UserName);

            if (user == null)
            {
                return false;
            }

            var joinRequest = Mapper.Map<JoinRequest>(model);

            joinRequest.User = user;

            await this.joinRequestsRepository.AddAsync(joinRequest);

            await this.joinRequestsRepository.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<JoinRequestServiceModel>> GetReceivedForUserAsync(string userName)
        {
            if (userName == null)
            {
                return null;
            }

            var requests = await this.joinRequestsRepository.All()
                .Where(r => r.Ride.Car.Owner.UserName == userName)
                .ProjectTo<JoinRequestServiceModel>()
                .ToArrayAsync();

            return requests;
        }

        public async Task<JoinRequestServiceModel> GetAsync(string id)
        {
            if (id == null)
            {
                return null;
            }

            var request = await this.joinRequestsRepository.All()
                .ProjectTo<JoinRequestServiceModel>()
                .SingleOrDefaultAsync(r => r.Id == id);

            return request;
        }

        public async Task<bool> AcceptAsync(string id)
        {
            if (id == null)
            {
                return false;
            }

            var request = await this.joinRequestsRepository.All()
                .SingleOrDefaultAsync(r => r.Id == id);

            var userRide = new UserRide
            {
                UserId = request.UserId,
                RideId = request.RideId,
            };

            await this.userRidesRepository.AddAsync(userRide);

            this.joinRequestsRepository.Remove(request);

            await this.joinRequestsRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (id == null)
            {
                return false;
            }

            var request = await this.joinRequestsRepository.All()
                .SingleOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return false;
            }

            this.joinRequestsRepository.Remove(request);

            await this.joinRequestsRepository.SaveChangesAsync();

            return true;
        }

        public bool CanUserSendJoinRequest(RideServiceModel rideServiceModel, string userName)
            => userName != null
               && rideServiceModel.Date >= DateTime.UtcNow
               && rideServiceModel.Participants.All(p => p.User.UserName != userName)
               && rideServiceModel.JoinRequests.All(r => r.User.UserName != userName)
               && rideServiceModel.Participants.Count <= rideServiceModel.AvailableSeats + 1;

        public async Task<bool> CanUserAccessRequestAsync(string id, string userName)
        {
            if (id == null || userName == null)
            {
                return false;
            }

            var request = await this.GetAsync(id);

            return request != null && request.Ride.Car.Owner.UserName == userName;
        }
    }
}