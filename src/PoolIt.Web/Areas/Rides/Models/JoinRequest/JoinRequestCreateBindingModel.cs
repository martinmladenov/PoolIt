namespace PoolIt.Web.Areas.Rides.Models.JoinRequest
{
    using System.ComponentModel.DataAnnotations;
    using Infrastructure.Mapping;
    using Services.Models;

    public class JoinRequestCreateBindingModel : IMapWith<JoinRequestServiceModel>
    {
        public string RideId { get; set; }
        
        public string RideTitle { get; set; }
        
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Message { get; set; }
    }
}