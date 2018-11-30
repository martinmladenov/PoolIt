namespace PoolIt.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Invitation
    {
        public string Id { get; set; }

        [Required]
        public string RideId { get; set; }

        public Ride Ride { get; set; }

        [Required]
        [StringLength(6)]
        public string Key { get; set; }
    }
}