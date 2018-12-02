namespace PoolIt.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UserEditBindingModel
    {
        public string Email { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 2)]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 2)]
        [Display(Name = "Last name")]
        public string LastName { get; set; }
    }
}