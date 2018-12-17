namespace PoolIt.Services.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Infrastructure.Mapping;
    using PoolIt.Models;

    public class CarManufacturerServiceModel : IMapWith<CarManufacturer>
    {
        public string Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        public ICollection<CarModelServiceModel> Models { get; set; }
    }
}