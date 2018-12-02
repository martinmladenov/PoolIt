namespace PoolIt.Web.Areas.Rides.Models.Ride
{
    using AutoMapper;
    using Infrastructure.Mapping;
    using Services.Models;

    public class UserRideViewModel : IHaveCustomMapping
    {
        public string UserId { get; set; }

        public string UserFullName { get; set; }

        public string UserEmail { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<UserRideServiceModel, UserRideViewModel>()
                .ForMember(dest => dest.UserFullName, opt =>
                    opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.UserEmail, opt =>
                    opt.MapFrom(src => src.User.Email));
        }
    }
}