namespace PoolIt.Web.Areas.Rides.Models.Invitation
{
    using Infrastructure.Mapping;
    using Microsoft.AspNetCore.Mvc;
    using Ride;
    using Services.Models;

    [Area("Rides")]
    public class InvitationViewModel : IMapWith<InvitationServiceModel>
    {
        public RideDetailsViewModel Ride { get; set; }

        public string Key { get; set; }
    }
}