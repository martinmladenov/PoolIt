namespace PoolIt.Web.Areas.Administration.Models
{
    using System.Collections.Generic;
    using Infrastructure.Mapping;
    using Services.Models;

    public class CarManufacturerAdminViewModel : IMapWith<CarManufacturerServiceModel>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public ICollection<CarModelAdminViewModel> Models { get; set; }
    }
}