namespace PoolIt.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Rides.Models.JoinRequest;
    using Services.Contracts;

    public class JoinRequestsController : AdministrationController
    {
        private readonly IJoinRequestsService joinRequestsService;

        public JoinRequestsController(IJoinRequestsService joinRequestsService)
        {
            this.joinRequestsService = joinRequestsService;
        }

        public async Task<IActionResult> Index()
        {
            var joinRequests = await this.joinRequestsService.GetAllAsync();

            return this.View(joinRequests.Select(Mapper.Map<JoinRequestListingViewModel>));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var result = await this.joinRequestsService.DeleteAsync(id);
            if (result)
            {
                this.Success(NotificationMessages.JoinRequestRefused);
            }
            else
            {
                this.Error(NotificationMessages.JoinRequestRefuseError);
            }

            return this.RedirectToAction("Index");
        }
    }
}