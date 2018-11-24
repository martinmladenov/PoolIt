namespace PoolIt.Services.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Infrastructure.Mapping;
    using PoolIt.Models;

    public class JoinRequestServiceModel : IMapWith<JoinRequest>
    {
        public string Id { get; set; }

        public string RideId { get; set; }

        public RideServiceModel Ride { get; set; }

        public string UserId { get; set; }

        [Required]
        public PoolItUserServiceModel User { get; set; }

        public DateTime SentOn { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Message { get; set; }
    }
}