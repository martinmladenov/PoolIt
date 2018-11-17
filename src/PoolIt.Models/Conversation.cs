namespace PoolIt.Models
{
    using System.Collections.Generic;

    public class Conversation
    {
        public string Id { get; set; }

        public Ride Ride { get; set; }

        public ICollection<Message> Messages { get; set; }
    }
}