namespace PoolIt.Web.Areas.Profile.Models.Profile
{
    using System.Collections.Generic;
    using Rides.Models.Ride;

    public class ProfileViewModel
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public IEnumerable<RideListingViewModel> UpcomingRides { get; set; }

        public IEnumerable<RideListingViewModel> PastRides { get; set; }
    }
}