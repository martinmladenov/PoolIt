namespace PoolIt.Services.Models
{
    using Infrastructure.Mapping;
    using PoolIt.Models;

    public class UserRideServiceModel : IMapWith<UserRide>
    {
        public string UserId { get; set; }

        public PoolItUserServiceModel User { get; set; }

        public string RideId { get; set; }

        public RideServiceModel Ride { get; set; }
    }
}