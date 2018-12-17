namespace PoolIt.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Rides.Models.Invitation;
    using Services.Contracts;

    public class InvitationsController : AdministrationController
    {
        private readonly IInvitationsService invitationsService;

        public InvitationsController(IInvitationsService invitationsService)
        {
            this.invitationsService = invitationsService;
        }

        public async Task<IActionResult> Index()
        {
            var invitations = await this.invitationsService.GetAllAsync();

            return this.View(invitations.Select(Mapper.Map<InvitationViewModel>));
        }

        public async Task<IActionResult> Delete(string key)
        {
            if (key == null)
            {
                return this.NotFound();
            }

            var result = await this.invitationsService.DeleteAsync(key);

            if (result)
            {
                this.Success(NotificationMessages.InvitationDeleted);
            }
            else
            {
                this.Error(NotificationMessages.InvitationDeleteError);
            }

            return this.RedirectToAction("Index");
        }
    }
}