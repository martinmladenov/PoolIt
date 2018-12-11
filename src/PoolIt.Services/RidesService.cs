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

        public RidesService(IRepository<Ride> ridesRepository, IRepository<PoolItUser> usersRepository,
            IRepository<Car> carsRepository)
        {
            this.ridesRepository = ridesRepository;
            this.usersRepository = usersRepository;
            this.carsRepository = carsRepository;
        }

        public async Task<string> CreateAsync(RideServiceModel model)
        {
            if (!this.IsEntityStateValid(model))
            {
                return null;
            }

            var ride = Mapper.Map<Ride>(model);

            ride.Conversation = new Conversation();

            var car = await this.carsRepository.All()
                .SingleOrDefaultAsync(c => c.Id == ride.CarId);

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
                .Where(r => r.Date > DateTime.Now &&
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
                .Where(r => r.Date > DateTime.Now &&
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
                .Where(r => r.Date <= DateTime.Now &&
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
            => rideServiceModel.Date > DateTime.Now
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
    }
}