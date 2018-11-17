namespace PoolIt.Services.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Infrastructure.Mapping;
    using PoolIt.Models;

    public class MessageServiceModel : IMapWith<Message>
    {
        public string Id { get; set; }

        [Required]
        public string ConversationId { get; set; }

        public ConversationServiceModel Conversation { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public PoolItUserServiceModel Author { get; set; }

        [Required]
        public DateTime SentOn { get; set; }
    }
}