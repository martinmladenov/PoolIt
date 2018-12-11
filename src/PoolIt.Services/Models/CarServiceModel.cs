namespace PoolIt.Services.Models
{
    using System.ComponentModel.DataAnnotations;
    using Infrastructure.Mapping;
    using PoolIt.Models;

    public class CarServiceModel : IMapWith<Car>
    {
        public string Id { get; set; }

        public string ModelId { get; set; }

        public CarModelServiceModel Model { get; set; }

        public string OwnerId { get; set; }

        public PoolItUserServiceModel Owner { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 2)]
        public string Colour { get; set; }

        [StringLength(300, MinimumLength = 3)]
        public string Details { get; set; }
    }
}