namespace PoolIt.Services.Models
{
    using Infrastructure.Mapping;
    using PoolIt.Models;

    public class ConversationServiceModel : IMapWith<Conversation>
    {
        public string Id { get; set; }

        public RideServiceModel Ride { get; set; }
    }
}