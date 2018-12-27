namespace PoolIt.Web.Areas.Administration.Models
{
    using System.Linq;
    using AutoMapper;
    using Infrastructure.Mapping;
    using PoolIt.Models;

    public class UserAdminListingModel : IHaveCustomMapping
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public int CarCount { get; set; }

        public int OrganisedRideCount { get; set; }

        public int SentRequestCount { get; set; }

        public int ParticipatingRideCount { get; set; }

        public bool IsAdmin { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<PoolItUser, UserAdminListingModel>()
                .ForMember(dest => dest.CarCount, opt =>
                    opt.MapFrom(src => src.Cars.Count))
                .ForMember(dest => dest.OrganisedRideCount, opt =>
                    opt.MapFrom(src => src.Cars.Sum(c => c.Rides.Count)))
                .ForMember(dest => dest.SentRequestCount, opt =>
                    opt.MapFrom(src => src.SentRequests.Count))
                .ForMember(dest => dest.ParticipatingRideCount, opt =>
                    opt.MapFrom(src => src.UserRides.Count));
        }
    }
}