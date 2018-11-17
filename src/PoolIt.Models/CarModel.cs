namespace PoolIt.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CarModel
    {
        public string Id { get; set; }

        [Required]
        public string ManufacturerId { get; set; }
        
        public CarManufacturer Manufacturer { get; set; }
        
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Model { get; set; }

        public ICollection<Car> Cars { get; set; }
    }
}