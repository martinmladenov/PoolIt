namespace PoolIt.Services.Models
{
    using Infrastructure.Mapping;
    using Microsoft.AspNetCore.Identity;
    using PoolIt.Models;

    public class PoolItUserServiceModel : IdentityUser, IMapWith<PoolItUser>
    {
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
    }
}
