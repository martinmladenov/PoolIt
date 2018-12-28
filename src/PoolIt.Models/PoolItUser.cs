namespace PoolIt.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Identity;

    public class PoolItUser : IdentityUser
    {
        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string LastName { get; set; }

        public ICollection<Car> Cars { get; set; }

        public ICollection<UserRide> UserRides { get; set; }

        public ICollection<JoinRequest> SentRequests { get; set; }

        public ICollection<Message> SentMessages { get; set; }

        public ICollection<ContactMessage> ContactMessages { get; set; }
    }
}