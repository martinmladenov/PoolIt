namespace PoolIt.Models
{
    using Microsoft.AspNetCore.Identity;

    public class PoolItUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
