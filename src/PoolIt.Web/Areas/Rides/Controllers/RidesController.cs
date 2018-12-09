namespace PoolIt.Web.Areas.Rides.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models.Ride;
    using Services.Contracts;
    using Services.Models;
    using Web.Controllers;

    [Area("Rides")]
    public class RidesController : BaseController
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

            var car = await this.carsService.GetAsync(model.CarId);

            if (car == null || !this.carsService.IsUserOwner(car, this.User?.Identity?.Name))
            {
                return this.Forbid();
            }

            var serviceModel = Mapper.Map<RideServiceModel>(model);

            var id = await this.ridesService.CreateAsync(serviceModel);

            return this.RedirectToAction("Details", new {id});
        }

        private async Task<IEnumerable<SelectListItem>> GetUserCars()
        {
            return (await this.carsService.GetAllForUserAsync(this.User.Identity.Name))
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
                    .GetAllUpcomingWithFreeSeatsAsync())
                .Select(Mapper.Map<RideListingViewModel>)
                .ToArray();

            return this.View(rides);
        }

        [Route("/ride/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var rideServiceModel = await this.ridesService.GetAsync(id);

            if (rideServiceModel == null
                || !this.ridesService.CanUserAccessRide(rideServiceModel, this.User?.Identity?.Name)
                && !this.User.IsInRole(GlobalConstants.AdminRoleName))
            {
                return this.NotFound();
            }

            var viewModel = Mapper.Map<RideDetailsViewModel>(rideServiceModel);

            return this.View(viewModel);
        }

        public async Task<IActionResult> Search(SearchBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            var rides = (await this.ridesService
                    .GetAllUpcomingWithFreeSeatsAsync())
                .Where(r => string.Equals(r.From, model.From, StringComparison.OrdinalIgnoreCase))
                .Select(Mapper.Map<RideListingViewModel>);

            model.FoundRides = rides;

            return this.View(model);
        }
    }
}