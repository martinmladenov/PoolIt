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
        private readonly IInvitationsService invitationsService;

        public RidesController(ICarsService carsService, IRidesService ridesService,
            IInvitationsService invitationsService)
        {
            this.carsService = carsService;
            this.ridesService = ridesService;
            this.invitationsService = invitationsService;
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

            model.Date = model.Date.ToUniversalTime();

            var car = await this.carsService.GetAsync(model.CarId);

            if (car == null || !this.carsService.IsUserOwner(car, this.User?.Identity?.Name))
            {
                this.Error(NotificationMessages.RideCreateError);
                return this.View();
            }

            var serviceModel = Mapper.Map<RideServiceModel>(model);

            var id = await this.ridesService.CreateAsync(serviceModel);

            if (id == null)
            {
                this.Error(NotificationMessages.RideCreateError);

                var userCars = await this.GetUserCars();

                model.OwnedCars = userCars;

                return this.View(model);
            }

            this.Success(NotificationMessages.RideCreated);

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

            var invitationKey = this.Request.Cookies[GlobalConstants.InvitationCookieKey];

            if (string.IsNullOrEmpty(invitationKey))
            {
                return this.View(viewModel);
            }

            var invitation = await this.invitationsService.GetAsync(invitationKey);

            if (invitation != null)
            {
                if (invitation.RideId == id)
                {
                    viewModel.InvitationKey = invitationKey;
                }
            }
            else
            {
                this.Response.Cookies.Delete(GlobalConstants.InvitationCookieKey);
            }

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

        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var serviceModel = await this.ridesService.GetAsync(id);

            if (serviceModel == null ||
                !this.ridesService.IsUserOrganiser(serviceModel, this.User?.Identity?.Name) &&
                !this.User.IsInRole(GlobalConstants.AdminRoleName))
            {
                return this.NotFound();
            }

            var bindingModel = Mapper.Map<RideEditBindingModel>(serviceModel);

            return this.View(bindingModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, RideEditBindingModel model)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var serviceModel = await this.ridesService.GetAsync(id);

            if (serviceModel == null ||
                !this.ridesService.IsUserOrganiser(serviceModel, this.User?.Identity?.Name) &&
                !this.User.IsInRole(GlobalConstants.AdminRoleName))
            {
                return this.NotFound();
            }

            serviceModel.Id = id;
            serviceModel.Title = model.Title;
            serviceModel.PhoneNumber = model.PhoneNumber;
            serviceModel.Notes = model.Notes;

            if (!this.ModelState.IsValid)
            {
                var bindingModel = Mapper.Map<RideEditBindingModel>(serviceModel);

                return this.View(bindingModel);
            }

            var result = await this.ridesService.UpdateAsync(serviceModel);
            if (result)
            {
                this.Success(NotificationMessages.RideEdited);
            }
            else
            {
                this.Error(NotificationMessages.RideEditError);
            }

            return this.RedirectToAction("Details", new {id});
        }
    }
}