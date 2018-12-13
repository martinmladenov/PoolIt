namespace PoolIt.Web.Areas.Conversations.Models
{
    using System;
    using AutoMapper;
    using Infrastructure.Mapping;
    using Services.Models;

    public class MessageViewModel : IHaveCustomMapping
    {
        public string AuthorId { get; set; }

        public string AuthorName { get; set; }

        public DateTime SentOn { get; set; }

        public string Content { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<MessageServiceModel, MessageViewModel>()
                .ForMember(dest => dest.AuthorName, opt =>
                    opt.MapFrom(src => src.Author.FirstName + " " + src.Author.LastName));
        }
    }
}