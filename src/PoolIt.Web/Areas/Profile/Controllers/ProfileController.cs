namespace PoolIt.Web.Areas.Profile.Controllers
{
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models.Profile;
    using PoolIt.Models;
    using Rides.Models.Ride;
    using Services.Contracts;
    using Web.Controllers;

    [Authorize]
    [Area("Profile")]
    public class ProfileController : BaseController
    {
        private const string PersonalDataFileName = "PoolIt_PersonalData_{0}_{1}.json";

        private readonly UserManager<PoolItUser> userManager;
        private readonly SignInManager<PoolItUser> signInManager;

        private readonly IRidesService ridesService;
        private readonly IPersonalDataService personalDataService;

        public ProfileController(UserManager<PoolItUser> userManager, IRidesService ridesService,
            IPersonalDataService personalDataService, SignInManager<PoolItUser> signInManager)
        {
            this.userManager = userManager;
            this.ridesService = ridesService;
            this.personalDataService = personalDataService;
            this.signInManager = signInManager;
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

        [HttpPost]
        public async Task<IActionResult> DownloadPersonalData(string password)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            var passwordValid = !await this.userManager.HasPasswordAsync(user) ||
                                await this.userManager.CheckPasswordAsync(user, password);

            if (!passwordValid)
            {
                this.Error(NotificationMessages.InvalidPassword);
                return this.RedirectToAction("Index");
            }

            var json = await this.personalDataService.GetPersonalDataForUserJson(user.Id);

            this.Response.Headers.Add("Content-Disposition",
                "attachment; filename=" + string.Format(PersonalDataFileName, user.FirstName, user.LastName));
            return new FileContentResult(Encoding.UTF8.GetBytes(json), "text/json");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(string password)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            var passwordValid = !await this.userManager.HasPasswordAsync(user) ||
                                await this.userManager.CheckPasswordAsync(user, password);

            if (!passwordValid)
            {
                this.Error(NotificationMessages.InvalidPassword);
                return this.RedirectToAction("Index");
            }

            var result = await this.personalDataService.DeleteUser(user.Id);

            if (!result)
            {
                this.Error(NotificationMessages.AccountDeleteError);
                return this.RedirectToAction("Index");
            }

            await this.signInManager.SignOutAsync();

            this.Success(NotificationMessages.AccountDeleted);
            return this.LocalRedirect("/");
        }
    }
}