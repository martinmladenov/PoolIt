namespace PoolIt.Web.Areas.Profile.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models.Car;
    using Services.Contracts;
    using Services.Models;
    using Web.Controllers;

    [Authorize]
    [Area("Profile")]
    public class CarsController : BaseController
    {
        private readonly IManufacturersService manufacturersService;
        private readonly ICarsService carsService;

        public CarsController(IManufacturersService manufacturersService, ICarsService carsService)
        {
            this.manufacturersService = manufacturersService;
            this.carsService = carsService;
        }

        public async Task<IActionResult> Create()
        {
            var manufacturers = await this.GetAllManufacturers();

            var model = new CarCreateBindingModel
            {
                Manufacturers = manufacturers
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CarCreateBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                var manufacturers = await this.GetAllManufacturers();

                model.Manufacturers = manufacturers;

                return this.View(model);
            }

            var serviceModel = Mapper.Map<CarServiceModel>(model);

            serviceModel.Owner = new PoolItUserServiceModel
            {
                UserName = this.User.Identity.Name
            };

            var result = await this.carsService.CreateAsync(serviceModel);

            if (!result)
            {
                this.Error(NotificationMessages.CarCreateError);
            }

            this.Success(NotificationMessages.CarCreated);

            return this.RedirectToAction("Create", "Rides", new {Area = "Rides"});
        }

        private async Task<IEnumerable<SelectListItem>> GetAllManufacturers()
        {
            var manufacturers = (await this.manufacturersService
                    .GetAllAsync())
                .Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id
                });

            return manufacturers;
        }

        public async Task<IActionResult> Index()
        {
            var models = (await this.carsService
                    .GetAllForUserAsync(this.User.Identity.Name))
                .Select(Mapper.Map<CarListingViewModel>);

            return this.View(models);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var serviceModel = await this.carsService.GetAsync(id);

            if (serviceModel == null ||
                !this.carsService.IsUserOwner(serviceModel, this.User?.Identity?.Name) &&
                !this.User.IsInRole(GlobalConstants.AdminRoleName))
            {
                return this.NotFound();
            }

            var bindingModel = Mapper.Map<CarEditBindingModel>(serviceModel);

            return this.View(bindingModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, CarEditBindingModel bindingModel)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var serviceModel = await this.carsService.GetAsync(id);

            if (serviceModel == null ||
                !this.carsService.IsUserOwner(serviceModel, this.User?.Identity?.Name) &&
                !this.User.IsInRole(GlobalConstants.AdminRoleName))
            {
                return this.NotFound();
            }

            if (!this.ModelState.IsValid)
            {
                bindingModel.ModelName = serviceModel.Model.Model;
                bindingModel.Manufacturer = serviceModel.Model.Manufacturer.Name;

                return this.View(bindingModel);
            }

            serviceModel.Id = id;
            serviceModel.Colour = bindingModel.Colour;
            serviceModel.Details = bindingModel.Details;

            var result = await this.carsService.UpdateAsync(serviceModel);
            if (result)
            {
                this.Success(NotificationMessages.CarEdited);
            }
            else
            {
                this.Error(NotificationMessages.CarEditError);
            }

            return this.RedirectToAction("Index");
        }
    }
}