namespace PoolIt.Web.Areas.Administration.Models
{
    using AutoMapper;
    using Infrastructure.Mapping;
    using Services.Models;

    public class ContactMessageAdminListingViewModel : IHaveCustomMapping
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public bool LoggedIn { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<ContactMessageServiceModel, ContactMessageAdminListingViewModel>()
                .ForMember(dest => dest.FullName, opt =>
                    opt.MapFrom(src => src.User != null
                        ? $"{src.User.FirstName} {src.User.LastName}"
                        : src.FullName))
                .ForMember(dest => dest.Email, opt =>
                    opt.MapFrom(src => src.User != null
                        ? src.User.Email
                        : src.Email))
                .ForMember(dest => dest.LoggedIn, opt =>
                    opt.MapFrom(src => src.User != null));
        }
    }
}