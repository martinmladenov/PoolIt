namespace PoolIt.Web.Areas.Conversations.Controllers
{
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using PoolIt.Models;
    using Services.Contracts;
    using Web.Controllers;

    [Authorize]
    [Area("Conversations")]
    public class ConversationsController : BaseController
    {
        private readonly IConversationsService conversationsService;
        private readonly UserManager<PoolItUser> userManager;
        private readonly IRidesService ridesService;

        public ConversationsController(IConversationsService conversationsService, UserManager<PoolItUser> userManager,
            IRidesService ridesService)
        {
            this.conversationsService = conversationsService;
            this.userManager = userManager;
            this.ridesService = ridesService;
        }

        [Route("/conversation/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var conversationServiceModel = await this.conversationsService.GetAsync(id);

            if (conversationServiceModel == null
                || !this.ridesService.IsUserParticipant(conversationServiceModel.Ride, this.User.Identity.Name)
                && !this.User.IsInRole(GlobalConstants.AdminRoleName))
            {
                return this.NotFound();
            }

            var viewModel = Mapper.Map<ConversationViewModel>(conversationServiceModel);

            viewModel.CurrentUserId = this.GetCurrentUserId();

            return this.View(viewModel);
        }

        private string GetCurrentUserId()
        {
            return this.userManager.GetUserId(this.User);
        }
    }
}