namespace PoolIt.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Car
    {
        public string Id { get; set; }

        public string ModelId { get; set; }

        public CarModel Model { get; set; }

        [Required]
        public string OwnerId { get; set; }

        public PoolItUser Owner { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 2)]
        public string Colour { get; set; }

        [StringLength(300, MinimumLength = 3)]
        public string Details { get; set; }

        public ICollection<Ride> Rides { get; set; }
    }
}