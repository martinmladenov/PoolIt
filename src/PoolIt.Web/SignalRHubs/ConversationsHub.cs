namespace PoolIt.Web.SignalRHubs
{
    using System;
    using System.Threading.Tasks;
    using System.Web;
    using Areas.Conversations.Models;
    using AutoMapper;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using Services.Contracts;
    using Services.Models;

    [Authorize]
    public class ConversationsHub : Hub
    {
        private readonly IConversationsService conversationsService;
        private readonly IRidesService ridesService;

        public ConversationsHub(IConversationsService conversationsService, IRidesService ridesService)
        {
            this.conversationsService = conversationsService;
            this.ridesService = ridesService;
        }

        public async Task Setup(string convId)
        {
            if (convId == null)
            {
                return;
            }

            var conversation = await this.conversationsService.GetAsync(convId);

            if (conversation == null ||
                !this.ridesService.IsUserParticipant(conversation.Ride, this.Context.User.Identity.Name) &&
                !this.Context.User.IsInRole(GlobalConstants.AdminRoleName))
            {
                this.Context.Abort();
                return;
            }

            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, convId);
        }

        public async Task SendMessage(string content, string convId)
        {
            if (convId == null || content == null || content.Length < 1)
            {
                return;
            }

            var conversation = await this.conversationsService.GetAsync(convId);

            if (conversation == null ||
                !this.ridesService.IsUserParticipant(conversation.Ride, this.Context.User.Identity.Name) &&
                !this.Context.User.IsInRole(GlobalConstants.AdminRoleName))
            {
                return;
            }

            var serviceModel = new MessageServiceModel
            {
                Author = new PoolItUserServiceModel
                {
                    UserName = this.Context.User.Identity.Name
                },
                Content = content,
                ConversationId = convId,
                SentOn = DateTime.UtcNow
            };

            var resultMsg = await this.conversationsService.SendMessageAsync(serviceModel);

            if (resultMsg == null)
            {
                return;
            }

            var viewModel = Mapper.Map<MessageViewModel>(resultMsg);

            viewModel.AuthorName = HttpUtility.HtmlEncode(viewModel.AuthorName);
            viewModel.Content = HttpUtility.HtmlEncode(viewModel.Content);

            await this.Clients.Groups(convId).SendAsync("MessageReceived", viewModel);
        }
    }
}