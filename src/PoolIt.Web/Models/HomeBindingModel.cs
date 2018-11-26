namespace PoolIt.Web.Models
{
    using System;
    using System.Collections.Generic;

    public class HomeBindingModel
    {
        public IEnumerable<RideListingViewModel> MyRides { get; set; }
    }
}