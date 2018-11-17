namespace PoolIt.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class JoinRequest
    {
        public string Id { get; set; }

        [Required]
        public string RideId { get; set; }

        public Ride Ride { get; set; }

        [Required]
        public string UserId { get; set; }

        public PoolItUser User { get; set; }

        [Required]
        public DateTime SentOn { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Message { get; set; }
    }
}