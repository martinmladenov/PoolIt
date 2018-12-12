namespace PoolIt.Services.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Infrastructure.Mapping;
    using PoolIt.Models;

    public class MessageServiceModel : IMapWith<Message>
    {
        public string Id { get; set; }

        public string ConversationId { get; set; }

        [Required]
        public ConversationServiceModel Conversation { get; set; }

        public string AuthorId { get; set; }

        [Required]
        public PoolItUserServiceModel Author { get; set; }

        [Required]
        public DateTime SentOn { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 1)]
        public string Content { get; set; }
    }
}