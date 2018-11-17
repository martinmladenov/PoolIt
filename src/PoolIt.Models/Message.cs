namespace PoolIt.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Message
    {
        public string Id { get; set; }

        [Required]
        public string ConversationId { get; set; }

        public Conversation Conversation { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public PoolItUser Author { get; set; }

        [Required]
        public DateTime SentOn { get; set; }
    }
}