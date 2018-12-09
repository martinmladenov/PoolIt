namespace PoolIt.Web.Controllers
{
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Areas.Rides.Models.Ride;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Contracts;

    public class HomeController : BaseController
    {
        private readonly IRidesService ridesService;

        public HomeController(IRidesService ridesService)
        {
            this.ridesService = ridesService;
        }

        public async Task<IActionResult> Index()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.View();
            }

            var rides = (await this.ridesService
                    .GetAllUpcomingForUserAsync(this.User.Identity.Name))
                .Select(Mapper.Map<RideListingViewModel>);

            var model = new HomeBindingModel
            {
                MyRides = rides
            };

            return this.View(model);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier});
        }
    }
}