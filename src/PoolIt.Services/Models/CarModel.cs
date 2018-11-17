namespace PoolIt.Services.Models
{
    using System.ComponentModel.DataAnnotations;
    using Infrastructure.Mapping;
    using PoolIt.Models;

    public class CarModelServiceModel : IMapWith<CarModel>
    {
        public string Id { get; set; }

        [Required]
        public string ManufacturerId { get; set; }
        
        public CarManufacturerServiceModel Manufacturer { get; set; }
        
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Model { get; set; }
    }
}