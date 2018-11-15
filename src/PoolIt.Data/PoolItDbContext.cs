namespace PoolIt.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class PoolItDbContext : IdentityDbContext<PoolItUser>
    {
        public PoolItDbContext(DbContextOptions<PoolItDbContext> options)
            : base(options)
        {
        }
    }
}
