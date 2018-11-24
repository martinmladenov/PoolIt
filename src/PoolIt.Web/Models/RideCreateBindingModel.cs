namespace PoolIt.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Infrastructure.Mapping;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services.Models;

    public class RideCreateBindingModel : IMapWith<RideServiceModel>, IValidatableObject
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Car")]
        public string CarId { get; set; }

        [Required]
        [Display(Name = "Date and time")]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string From { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string To { get; set; }

        [Required]
        [Range(1, 6)]
        [Display(Name = "Available seats")]
        public int AvailableSeats { get; set; }

        [RegularExpression(@"^(0|(\+\d{1,3}))\d{6,12}$", ErrorMessage = "Enter a valid phone number")]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [StringLength(300, MinimumLength = 3)]
        [Display(Name = "Additional notes")]
        public string Notes { get; set; }

        public IEnumerable<SelectListItem> OwnedCars { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.Date <= DateTime.Now)
            {
                yield return new ValidationResult("The date and time must be after the current time", new[] {"Date"});
            }
        }
    }
}