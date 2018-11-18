namespace PoolIt.Web.Models
{
    using Infrastructure.Mapping;
    using Services.Models;

    public class CarManufacturerViewModel : IMapWith<CarManufacturerServiceModel>
    {
        public string Name { get; set; }
    }
}