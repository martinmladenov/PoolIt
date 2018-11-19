namespace PoolIt.Services
{
    using System.Threading.Tasks;
    using AutoMapper;
    using Contracts;
    using Data;
    using Models;
    using PoolIt.Models;

    public class RidesService : DataService, IRidesService
    {
        public RidesService(PoolItDbContext context) : base(context)
        {
        }

        public async Task<string> Create(RideServiceModel model)
        {
            if (!this.IsEntityStateValid(model))
            {
                return null;
            }

            var ride = Mapper.Map<Ride>(model);
            
            ride.Conversation = new Conversation();

            await this.context.Rides.AddAsync(ride);

            await this.context.SaveChangesAsync();

            return ride.Id;
        }
    }
}