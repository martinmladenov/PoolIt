namespace PoolIt.Web.Models
{
    using System.Collections.Generic;
    using Areas.Rides.Models.Ride;

    public class HomeBindingModel
    {
        public IEnumerable<RideListingViewModel> MyRides { get; set; }
    }
}