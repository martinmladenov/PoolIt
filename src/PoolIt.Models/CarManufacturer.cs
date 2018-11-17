namespace PoolIt.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CarManufacturer
    {
        public string Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        public ICollection<CarModel> Models { get; set; }
    }
}