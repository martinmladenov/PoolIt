namespace PoolIt.Web.Areas.Conversations.Models
{
    using System.Collections.Generic;
    using Infrastructure.Mapping;
    using Rides.Models.Ride;
    using Services.Models;

    public class ConversationViewModel : IMapWith<ConversationServiceModel>
    {
        public string Id { get; set; }

        public RideDetailsViewModel Ride { get; set; }

        public IEnumerable<MessageViewModel> Messages { get; set; }

        public string CurrentUserId { get; set; }
    }
}