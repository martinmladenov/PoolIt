namespace PoolIt.Models
{
    public class UserRide
    {
        public string UserId { get; set; }

        public PoolItUser User { get; set; }

        public string RideId { get; set; }

        public Ride Ride { get; set; }
    }
}