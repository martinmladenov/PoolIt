namespace PoolIt.Web.Areas.Profile.Models.CarModel
{
    using System.ComponentModel.DataAnnotations;

    public class CarModelCreateBindingModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Manufacturer { get; set; }

        [Required]
        [Display(Name = "Model")]
        [StringLength(50, MinimumLength = 3)]
        public string CarModel { get; set; }
    }
}