namespace PoolIt.Web.Models
{
    using System;
    using AutoMapper;
    using Infrastructure.Mapping;
    using Services.Models;

    public class RideListingViewModel : IHaveCustomMapping
    {
        public string Id { get; set; }
        
        public string Title { get; set; }

        public string CarModel { get; set; }

        public DateTime Date { get; set; }

        public string From { get; set; }
        
        public string To { get; set; }

        public int RemainingSeats { get; set; }
        
        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<RideServiceModel, RideListingViewModel>()
                .ForMember(dest => dest.CarModel, opt => 
                    opt.MapFrom(src => $"{src.Car.Model.Manufacturer.Name} {src.Car.Model.Model}"))
                .ForMember(dest => dest.RemainingSeats, opt =>
                    opt.MapFrom(src => src.AvailableSeats - src.Participants.Count + 1));
        }
    }
}