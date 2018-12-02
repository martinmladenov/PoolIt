namespace PoolIt.Web.Areas.Account.Models.Details
{
    using System.ComponentModel.DataAnnotations;

    public class UserSetPasswordBindingModel
    {
        public string Email { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}