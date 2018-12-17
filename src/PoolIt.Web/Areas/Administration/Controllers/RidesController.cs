namespace PoolIt.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Rides.Models.Ride;
    using Services.Contracts;

    public class RidesController : AdministrationController
    {
        private readonly IRidesService ridesService;

        public RidesController(IRidesService ridesService)
        {
            this.ridesService = ridesService;
        }

        public async Task<IActionResult> Index()
        {
            var allRides = await this.ridesService.GetAllAsync();

            return this.View(allRides.Select(Mapper.Map<RideDetailsViewModel>));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var result = await this.ridesService.DeleteAsync(id);

            if (result)
            {
                this.Success(NotificationMessages.RideDeleted);
            }
            else
            {
                this.Error(NotificationMessages.RideDeleteError);
            }

            return this.RedirectToAction("Index");
        }
    }
}