namespace PoolIt.Services
{
    using System;
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

    public class RidesService : DataService, IRidesService
    {
        private readonly ICarsService carsService;

        public RidesService(PoolItDbContext context, ICarsService carsService) : base(context)
        {
            this.carsService = carsService;
        }

        public async Task<string> Create(RideServiceModel model)
        {
            if (!this.IsEntityStateValid(model))
            {
                return null;
            }

            var ride = Mapper.Map<Ride>(model);

            ride.Conversation = new Conversation();

            var organiser = await this.carsService.Get(ride.CarId);

            ride.Participants = new List<UserRide>
            {
                new UserRide
                {
                    UserId = organiser.OwnerId
                }
            };

            await this.context.Rides.AddAsync(ride);

            await this.context.SaveChangesAsync();

            return ride.Id;
        }

        public async Task<IEnumerable<RideServiceModel>> GetAllUpcomingWithFreeSeats()
        {
            var rides = await this.context.Rides
                .Where(r => r.Date > DateTime.Now &&
                            r.AvailableSeats > r.Participants.Count - 1)
                .OrderBy(r => r.Date)
                .ProjectTo<RideServiceModel>()
                .ToArrayAsync();

            return rides;
        }
    }
}