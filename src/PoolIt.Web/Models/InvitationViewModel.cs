namespace PoolIt.Web.Models
{
    using Infrastructure.Mapping;
    using Services.Models;

    public class InvitationViewModel : IMapWith<InvitationServiceModel>
    {
        public RideDetailsViewModel Ride { get; set; }

        public string Key { get; set; }
    }
}