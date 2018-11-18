namespace PoolIt.Web.Models
{
    using Infrastructure.Mapping;
    using Services.Models;

    public class CarListingViewModel : IMapWith<CarServiceModel>
    {
        public CarModelListingViewModel Model { get; set; }

        public string Colour { get; set; }

        public string Details { get; set; }
    }
}