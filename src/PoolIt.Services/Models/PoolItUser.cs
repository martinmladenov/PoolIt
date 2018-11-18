namespace PoolIt.Services.Models
{
    using System.ComponentModel.DataAnnotations;
    using Infrastructure.Mapping;
    using Microsoft.AspNetCore.Identity;
    using PoolIt.Models;

    public class PoolItUserServiceModel : IdentityUser, IMapWith<PoolItUser>
    {
        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string FirstName { get; set; }
        
        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string LastName { get; set; }
    }
}
