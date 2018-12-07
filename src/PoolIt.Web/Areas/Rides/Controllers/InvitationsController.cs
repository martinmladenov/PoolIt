namespace PoolIt.Web.Areas.Rides.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.Invitation;
    using Services.Contracts;

    [Area("Rides")]
    public class InvitationsController : Controller
    {
        private readonly IInvitationsService invitationsService;
        private readonly IRidesService ridesService;

        public InvitationsController(IInvitationsService invitationsService, IRidesService ridesService)
        {
            this.invitationsService = invitationsService;
            this.ridesService = ridesService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Generate(string rideId)
        {
            if (rideId == null)
            {
                return this.NotFound();
            }

            var ride = await this.ridesService.GetAsync(rideId);

            if (ride == null || !this.ridesService.IsUserOrganiser(ride, this.User?.Identity?.Name))
            {
                return this.NotFound();
            }

            var invitationKey = await this.invitationsService.GenerateAsync(rideId);

            return this.RedirectToAction("Details", new {invitationKey});
        }

        [Route("/invitation/{invitationKey}")]
        public async Task<IActionResult> Details(string invitationKey)
        {
            if (invitationKey == null)
            {
                return this.NotFound();
            }

            var serviceModel = await this.invitationsService.GetAsync(invitationKey);

            if (serviceModel == null)
            {
                return this.NotFound();
            }

            var viewModel = Mapper.Map<InvitationViewModel>(serviceModel);

            return this.View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Accept(string invitationKey)
        {
            if (invitationKey == null)
            {
                return this.NotFound();
            }

            var serviceModel = await this.invitationsService.GetAsync(invitationKey);

            if (serviceModel == null)
            {
                return this.NotFound();
            }

            if (serviceModel.Ride.Participants.All(p => p.User.UserName != this.User.Identity.Name))
            {
                await this.invitationsService.AcceptAsync(this.User.Identity.Name, invitationKey);
            }

            return this.RedirectToAction("Details", "Rides", new {id = serviceModel.Ride.Id});
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(string invitationKey)
        {
            if (invitationKey == null)
            {
                return this.NotFound();
            }

            var serviceModel = await this.invitationsService.GetAsync(invitationKey);

            if (serviceModel == null ||
                !this.ridesService.IsUserOrganiser(serviceModel.Ride, this.User?.Identity?.Name))
            {
                return this.NotFound();
            }

            await this.invitationsService.DeleteAsync(invitationKey);

            return this.RedirectToAction("Details", "Rides", new {id = serviceModel.Ride.Id});
        }
    }
}