namespace PoolIt.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ContactMessage
    {
        public string Id { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string FullName { get; set; }

        [StringLength(50, MinimumLength = 5)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Subject { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 5)]
        public string Message { get; set; }

        public string UserId { get; set; }

        public PoolItUser User { get; set; }
    }
}