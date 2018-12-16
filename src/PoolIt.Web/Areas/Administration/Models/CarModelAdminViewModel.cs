namespace PoolIt.Web.Areas.Administration.Models
{
    using AutoMapper;
    using Infrastructure.Mapping;
    using Services.Models;

    public class CarModelAdminViewModel : IHaveCustomMapping
    {
        public string Id { get; set; }

        public string Model { get; set; }

        public int CarCount { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<CarModelServiceModel, CarModelAdminViewModel>()
                .ForMember(dest => dest.CarCount, opt =>
                    opt.MapFrom(src => src.Cars.Count));
        }
    }
}