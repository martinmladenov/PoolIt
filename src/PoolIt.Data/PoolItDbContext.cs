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

        public DbSet<CarManufacturer> CarManufacturers { get; set; }
        
        public DbSet<CarModel> CarModels { get; set; }
        
        public DbSet<Car> Cars { get; set; }

        public DbSet<Conversation> Conversations { get; set; }

        public DbSet<JoinRequest> JoinRequests { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Ride> Rides { get; set; }

        public DbSet<UserRide> UserRides { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserRide>()
                .HasKey(ur => new {ur.UserId, ur.RideId});

            builder.Entity<JoinRequest>()
                .HasOne(j => j.User)
                .WithMany(u => u.SentRequests)
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<UserRide>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRides)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            base.OnModelCreating(builder);
        }
    }
}
