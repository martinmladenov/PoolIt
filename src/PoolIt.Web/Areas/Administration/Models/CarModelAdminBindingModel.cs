namespace PoolIt.Web.Areas.Administration.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using Infrastructure.Mapping;
    using Services.Models;

    public class CarModelAdminBindingModel : IHaveCustomMapping
    {
        [Required]
        [DisplayName("Model")]
        [StringLength(50, MinimumLength = 3)]
        public string ModelName { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<CarModelAdminBindingModel, CarModelServiceModel>()
                .ForMember(dest => dest.Model, opt =>
                    opt.MapFrom(src => src.ModelName));
        }
    }
}