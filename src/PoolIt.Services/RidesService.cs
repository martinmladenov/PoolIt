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

    public class RidesService : BaseService, IRidesService
    {
        private readonly IRepository<Ride> ridesRepository;
        private readonly IRepository<PoolItUser> usersRepository;
        private readonly IRepository<Car> carsRepository;
        private readonly IRepository<Conversation> conversationsRepository;

        public RidesService(IRepository<Ride> ridesRepository, IRepository<PoolItUser> usersRepository,
            IRepository<Car> carsRepository, IRepository<Conversation> conversationsRepository)
        {
            this.ridesRepository = ridesRepository;
            this.usersRepository = usersRepository;
            this.carsRepository = carsRepository;
            this.conversationsRepository = conversationsRepository;
        }

        public async Task<string> CreateAsync(RideServiceModel model)
        {
            if (!this.IsEntityStateValid(model))
            {
                return null;
            }

            var car = await this.carsRepository.All()
                .SingleOrDefaultAsync(c => c.Id == model.CarId);

            if (car == null)
            {
                return null;
            }

            var ride = Mapper.Map<Ride>(model);

            ride.Conversation = new Conversation();

            ride.Participants = new List<UserRide>
            {
                new UserRide
                {
                    UserId = car.OwnerId
                }
            };

            await this.ridesRepository.AddAsync(ride);

            await this.ridesRepository.SaveChangesAsync();

            return ride.Id;
        }

        public async Task<IEnumerable<RideServiceModel>> GetAllUpcomingWithFreeSeatsAsync()
        {
            var rides = await this.ridesRepository.All()
                .Where(r => r.Date > DateTime.UtcNow &&
                            r.AvailableSeats > r.Participants.Count - 1)
                .OrderBy(r => r.Date)
                .ProjectTo<RideServiceModel>()
                .ToArrayAsync();

            return rides;
        }

        public async Task<IEnumerable<RideServiceModel>> GetAllUpcomingForUserAsync(string userName)
        {
            var user = await this.usersRepository.All().SingleOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                return null;
            }

            var userRides = await this.ridesRepository.All()
                .Where(r => r.Date > DateTime.UtcNow &&
                            r.Participants.Any(p => p.UserId == user.Id))
                .OrderBy(r => r.Date)
                .ProjectTo<RideServiceModel>()
                .ToArrayAsync();

            return userRides;
        }

        public async Task<IEnumerable<RideServiceModel>> GetAllPastForUserAsync(string userName)
        {
            var user = await this.usersRepository.All().SingleOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                return null;
            }

            var userRides = await this.ridesRepository.All()
                .Where(r => r.Date <= DateTime.UtcNow &&
                            r.Participants.Any(p => p.UserId == user.Id))
                .OrderByDescending(r => r.Date)
                .ProjectTo<RideServiceModel>()
                .ToArrayAsync();

            return userRides;
        }

        public async Task<RideServiceModel> GetAsync(string id)
        {
            if (id == null)
            {
                return null;
            }

            var ride = await this.ridesRepository.All()
                .ProjectTo<RideServiceModel>()
                .SingleOrDefaultAsync(r => r.Id == id);

            return ride;
        }

        public bool CanUserAccessRide(RideServiceModel rideServiceModel, string userName)
            => rideServiceModel.Date > DateTime.UtcNow
               || userName != null
               && rideServiceModel.Participants.Any(p => p.User.UserName == userName);

        public bool IsUserOrganiser(RideServiceModel rideServiceModel, string userName)
        {
            return userName != null && rideServiceModel.Car.Owner.UserName == userName;
        }

        public bool IsUserParticipant(RideServiceModel rideServiceModel, string userName)
            => userName != null
               && rideServiceModel.Participants.Any(p => p.User.UserName == userName);

        public async Task<bool> UpdateAsync(RideServiceModel model)
        {
            if (!this.IsEntityStateValid(model) || model.Id == null)
            {
                return false;
            }

            var ride = await this.ridesRepository.All().SingleOrDefaultAsync(c => c.Id == model.Id);

            if (ride == null)
            {
                return false;
            }

            ride.Title = model.Title;
            ride.PhoneNumber = model.PhoneNumber;
            ride.Notes = model.Notes;

            this.ridesRepository.Update(ride);
            await this.ridesRepository.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<RideServiceModel>> GetAllAsync()
        {
            var rides = await this.ridesRepository.All()
                .ProjectTo<RideServiceModel>()
                .ToArrayAsync();

            return rides;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (id == null)
            {
                return false;
            }

            var ride = await this.ridesRepository.All()
                .Include(r => r.Conversation)
                .SingleOrDefaultAsync(r => r.Id == id);

            if (ride == null)
            {
                return false;
            }

            this.ridesRepository.Remove(ride);

            this.conversationsRepository.Remove(ride.Conversation);

            await this.ridesRepository.SaveChangesAsync();

            return true;
        }
    }
}