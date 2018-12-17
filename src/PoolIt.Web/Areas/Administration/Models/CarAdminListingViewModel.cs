namespace PoolIt.Web.Areas.Administration.Models
{
    using AutoMapper;
    using Infrastructure.Mapping;
    using Profile.Models.CarModel;
    using Services.Models;

    public class CarAdminListingViewModel : IHaveCustomMapping
    {
        public string Id { get; set; }

        public string OwnerFullName { get; set; }

        public string OwnerEmail { get; set; }

        public int RidesCount { get; set; }

        public CarModelListingViewModel Model { get; set; }

        public string Colour { get; set; }

        public string Details { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<CarServiceModel, CarAdminListingViewModel>()
                .ForMember(dest => dest.OwnerFullName, opt =>
                    opt.MapFrom(src => $"{src.Owner.FirstName} {src.Owner.LastName}"))
                .ForMember(dest => dest.OwnerEmail, opt =>
                    opt.MapFrom(src => src.Owner.Email))
                .ForMember(dest => dest.RidesCount, opt =>
                    opt.MapFrom(src => src.Rides.Count));
        }
    }
}