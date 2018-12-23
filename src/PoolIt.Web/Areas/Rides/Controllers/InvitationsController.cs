namespace PoolIt.Web.Areas.Rides.Controllers
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models.Invitation;
    using Services.Contracts;
    using Web.Controllers;

    [Area("Rides")]
    public class InvitationsController : BaseController
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

            this.Success(NotificationMessages.InvitationGenerated);

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

            if (!this.ridesService.IsUserOrganiser(serviceModel.Ride, this.User?.Identity?.Name))
            {
                this.Response.Cookies.Append(GlobalConstants.InvitationCookieKey, invitationKey, new CookieOptions
                {
                    Secure = true,
                    IsEssential = false,
                    Expires = DateTimeOffset.UtcNow.AddDays(14)
                });
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

            if (!this.ridesService.IsUserParticipant(serviceModel.Ride, this.User?.Identity?.Name))
            {
                var result = await this.invitationsService.AcceptAsync(this.User.Identity.Name, invitationKey);

                if (result)
                {
                    this.Success(NotificationMessages.InvitationAccepted);
                }
                else
                {
                    this.Error(NotificationMessages.InvitationAcceptNoPermission);
                }
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

            var result = await this.invitationsService.DeleteAsync(invitationKey);

            if (result)
            {
                this.Success(NotificationMessages.InvitationDeleted);
            }
            else
            {
                this.Error(NotificationMessages.InvitationDeleteError);
            }

            return this.RedirectToAction("Details", "Rides", new {id = serviceModel.Ride.Id});
        }
    }
}