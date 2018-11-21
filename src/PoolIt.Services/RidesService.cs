namespace PoolIt.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Contracts;
    using Data;
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
    }
}