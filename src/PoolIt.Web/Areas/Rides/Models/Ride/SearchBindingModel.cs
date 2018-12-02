namespace PoolIt.Web.Areas.Rides.Models.Ride
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class SearchBindingModel
    {
        [Required(ErrorMessage = "Please enter a starting location")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters")]
        [Display(Name = "Where are you travelling from?")]
        public string From { get; set; }

        public IEnumerable<RideListingViewModel> FoundRides { get; set; }
    }
}