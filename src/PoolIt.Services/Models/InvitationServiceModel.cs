namespace PoolIt.Services.Models
{
    using System.ComponentModel.DataAnnotations;
    using Infrastructure.Mapping;
    using PoolIt.Models;

    public class InvitationServiceModel : IMapWith<Invitation>
    {
        public string Id { get; set; }

        [Required]
        public string RideId { get; set; }

        public RideServiceModel Ride { get; set; }

        [Required]
        [StringLength(6)]
        public string Key { get; set; }
    }
}