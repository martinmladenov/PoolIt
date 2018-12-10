namespace PoolIt.Web.Areas.Rides.Models.Ride
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using Infrastructure.Mapping;
    using Services.Models;

    public class RideEditBindingModel : IHaveCustomMapping
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        public string Car { get; set; }

        public DateTime Date { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public int AvailableSeats { get; set; }

        [RegularExpression(@"^(0|(\+\d{1,3}))\d{6,12}$", ErrorMessage = "Enter a valid phone number")]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [StringLength(300, MinimumLength = 3)]
        [Display(Name = "Additional notes")]
        public string Notes { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<RideServiceModel, RideEditBindingModel>()
                .ForMember(dest => dest.Car, opt =>
                    opt.MapFrom(src => $"{src.Car.Colour} {src.Car.Model.Manufacturer.Name} {src.Car.Model.Model}"));
        }
    }
}