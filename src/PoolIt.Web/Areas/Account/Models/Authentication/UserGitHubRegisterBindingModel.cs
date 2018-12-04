namespace PoolIt.Web.Areas.Account.Models.Authentication
{
    using System.ComponentModel.DataAnnotations;
    using Web.Models;

    public class UserGitHubRegisterBindingModel : BaseReCaptchaModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
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