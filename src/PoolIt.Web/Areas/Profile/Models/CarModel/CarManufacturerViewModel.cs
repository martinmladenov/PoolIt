namespace PoolIt.Web.Areas.Profile.Models.CarModel
{
    using Infrastructure.Mapping;
    using Services.Models;

    public class CarManufacturerViewModel : IMapWith<CarManufacturerServiceModel>
    {
        public string Name { get; set; }
    }
}