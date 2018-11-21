namespace PoolIt.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models;
    using Services.Contracts;
    using Services.Models;

    public class RidesController : Controller
    {
        private readonly ICarsService carsService;
        private readonly IRidesService ridesService;

        public RidesController(ICarsService carsService, IRidesService ridesService)
        {
            this.carsService = carsService;
            this.ridesService = ridesService;
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            var userCars = await this.GetUserCars();

            var model = new RideCreateBindingModel
            {
                OwnedCars = userCars
            };

            return this.View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(RideCreateBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                var userCars = await this.GetUserCars();

                model.OwnedCars = userCars;

                return this.View(model);
            }

            var car = await this.carsService.Get(model.CarId);

            if (car == null || car.Owner.Email != this.User.Identity.Name)
            {
                return this.Forbid();
            }

            var serviceModel = Mapper.Map<RideServiceModel>(model);

            await this.ridesService.Create(serviceModel);

            return this.RedirectToAction("Index", "Home");
        }

        private async Task<IEnumerable<SelectListItem>> GetUserCars()
        {
            return (await this.carsService.GetAllForUser(this.User.Identity.Name))
                .Select(c => new SelectListItem
                {
                    Text = $"{c.Colour} {c.Model.Manufacturer.Name} {c.Model.Model}",
                    Value = c.Id
                });
        }

        [Route("/browse")]
        public async Task<IActionResult> All()
        {
            var rides = (await this.ridesService
                    .GetAllUpcomingWithFreeSeats())
                .Select(Mapper.Map<RideListingViewModel>)
                .ToArray();

            return this.View(rides);
        }
    }
}