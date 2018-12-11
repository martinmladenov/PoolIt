namespace PoolIt.Web.Areas.Profile.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models.Profile;
    using PoolIt.Models;
    using Rides.Models.Ride;
    using Services.Contracts;

    [Authorize]
    [Area("Profile")]
    public class ProfileController : Controller
    {
        private readonly UserManager<PoolItUser> userManager;
        private readonly IRidesService ridesService;

        public ProfileController(UserManager<PoolItUser> userManager, IRidesService ridesService)
        {
            this.userManager = userManager;
            this.ridesService = ridesService;
        }

        [Route("/profile")]
        public async Task<IActionResult> Index()
        {
            var user = await this.userManager.GetUserAsync(this.User);

            var upcomingRides = (await this.ridesService
                    .GetAllUpcomingForUserAsync(this.User?.Identity?.Name))
                .Select(Mapper.Map<RideListingViewModel>);

            var pastRides = (await this.ridesService
                    .GetAllPastForUserAsync(this.User?.Identity?.Name))
                .Select(Mapper.Map<RideListingViewModel>);

            var model = new ProfileViewModel
            {
                FullName = user.FirstName + " " + user.LastName,
                Email = user.Email,
                UpcomingRides = upcomingRides,
                PastRides = pastRides
            };

            return this.View(model);
        }
    }
}