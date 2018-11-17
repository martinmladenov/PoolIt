namespace PoolIt.Models
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;

    public class PoolItUser : IdentityUser
    {
        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public ICollection<Car> Cars { get; set; }

        public ICollection<UserRide> UserRides { get; set; }
        
        public ICollection<JoinRequest> SentRequests { get; set; }

        public ICollection<Message> SentMessages { get; set; }
    }
}
