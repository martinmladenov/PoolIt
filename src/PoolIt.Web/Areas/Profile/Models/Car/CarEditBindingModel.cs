namespace PoolIt.Web.Areas.Profile.Models.Car
{
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using Infrastructure.Mapping;
    using Services.Models;

    public class CarEditBindingModel : IHaveCustomMapping
    {
        public string Manufacturer { get; set; }

        public string ModelName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 2)]
        public string Colour { get; set; }

        [StringLength(300, MinimumLength = 3)]
        public string Details { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<CarServiceModel, CarEditBindingModel>()
                .ForMember(dest => dest.Manufacturer, opt =>
                    opt.MapFrom(src => src.Model.Manufacturer.Name))
                .ForMember(dest => dest.ModelName, opt =>
                    opt.MapFrom(src => src.Model.Model));
        }
    }
}