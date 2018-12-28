namespace PoolIt.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using Infrastructure.Mapping;
    using Services.Models;

    public class ContactMessageBindingModel : IMapWith<ContactMessageServiceModel>
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(50, MinimumLength = 5)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Subject { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 5)]
        public string Message { get; set; }
    }
}