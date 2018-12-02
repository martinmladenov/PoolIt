namespace PoolIt.Web.Areas.Profile.Models.CarModel
{
    using Infrastructure.Mapping;
    using Services.Models;

    public class CarModelListingViewModel : IMapWith<CarModelServiceModel>
    {
        public CarManufacturerViewModel Manufacturer { get; set; }
        
        public string Model { get; set; }
    }
}