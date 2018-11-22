namespace PoolIt.Web.Models
{
    using System;
    using AutoMapper;
    using Infrastructure.Mapping;
    using Services.Models;

    public class JoinRequestDetailsViewModel : IHaveCustomMapping
    {
        public string Id { get; set; }

        public string RideId { get; set; }

        public RideListingViewModel Ride { get; set; }

        public string UserId { get; set; }

        public string UserFullName { get; set; }

        public string UserEmail { get; set; }

        public DateTime SentOn { get; set; }

        public string Message { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<JoinRequestServiceModel, JoinRequestDetailsViewModel>()
                .ForMember(dest => dest.UserFullName, opt =>
                    opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.UserEmail, opt =>
                    opt.MapFrom(src => src.User.Email));
        }
    }
}