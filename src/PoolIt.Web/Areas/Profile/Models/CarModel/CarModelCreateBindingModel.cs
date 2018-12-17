namespace PoolIt.Web.Areas.Profile.Models.CarModel
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CarModelCreateBindingModel
    {
        public IEnumerable<SelectListItem> Manufacturers { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Manufacturer { get; set; }

        [Required]
        [Display(Name = "Model")]
        [StringLength(50, MinimumLength = 3)]
        public string CarModel { get; set; }
    }
}