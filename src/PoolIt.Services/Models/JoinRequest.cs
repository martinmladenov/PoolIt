namespace PoolIt.Services.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Infrastructure.Mapping;
    using PoolIt.Models;

    public class JoinRequestServiceModel : IMapWith<JoinRequest>
    {
        public string Id { get; set; }

        [Required]
        public string RideId { get; set; }

        public RideServiceModel Ride { get; set; }

        [Required]
        public string UserId { get; set; }

        public PoolItUserServiceModel User { get; set; }

        [Required]
        public DateTime SentOn { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Message { get; set; }
    }
}