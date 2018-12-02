namespace PoolIt.Web.Areas.Profile.Models.Car
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Infrastructure.Mapping;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services.Models;

    public class CarEditBindingModel : IMapWith<CarServiceModel>
    {
        public IEnumerable<SelectListItem> Manufacturers { get; set; }
        
        [Required]
        [Display(Name = "Model")]
        public string ModelId { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 2)]
        public string Colour { get; set; }

        [StringLength(300, MinimumLength = 3)]
        public string Details { get; set; }
        
        
    }
}