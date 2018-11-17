namespace PoolIt.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Ride
    {
        public string Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }
        
        [Required]
        public string CarId { get; set; }

        public Car Car { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 10)]
        public string From { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 10)]
        public string To { get; set; }

        [Required]
        [Range(1, 6)]
        public int AvailableSeats { get; set; }

        [RegularExpression(@"^(0|(\+\d{1,3}))\d{6,12}$")]
        public string PhoneNumber { get; set; }
        
        [StringLength(300, MinimumLength = 3)]
        public string Notes { get; set; }

        [Required]
        public string ConversationId { get; set; }

        public Conversation Conversation { get; set; }

        public ICollection<UserRide> Participants { get; set; }

        public ICollection<JoinRequest> JoinRequests { get; set; }
    }
}