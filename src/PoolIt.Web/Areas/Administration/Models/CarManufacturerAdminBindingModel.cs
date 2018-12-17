namespace PoolIt.Web.Areas.Administration.Models
{
    using System.ComponentModel.DataAnnotations;
    using Infrastructure.Mapping;
    using Services.Models;

    public class CarManufacturerAdminBindingModel : IMapWith<CarManufacturerServiceModel>
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
    }
}