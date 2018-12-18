namespace PoolIt.Web.Areas.Profile.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models.CarModel;
    using Services.Contracts;
    using Services.Models;
    using Web.Controllers;

    [Authorize]
    [Area("Profile")]
    public class ModelsController : BaseController
    {
        private readonly IManufacturersService manufacturersService;
        private readonly IModelsService modelsService;

        public ModelsController(IManufacturersService manufacturersService, IModelsService modelsService)
        {
            this.manufacturersService = manufacturersService;
            this.modelsService = modelsService;
        }

        public async Task<IActionResult> Create()
        {
            var manufacturers = await this.GetAllManufacturers();

            var model = new CarModelCreateBindingModel()
            {
                Manufacturers = manufacturers
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CarModelCreateBindingModel model, string returnUrl)
        {
            if (!this.ModelState.IsValid)
            {
                model.Manufacturers = await this.GetAllManufacturers();

                return this.View(model);
            }

            var manufacturer = await this.manufacturersService.GetByNameAsync(model.Manufacturer)
                               ?? new CarManufacturerServiceModel
                               {
                                   Name = model.Manufacturer
                               };

            var carModel = new CarModelServiceModel
            {
                ManufacturerId = manufacturer.Id,
                Manufacturer = manufacturer,
                Model = model.CarModel
            };

            returnUrl = returnUrl ?? "/";

            if (await this.modelsService.ExistsAsync(carModel))
            {
                this.Success(NotificationMessages.ModelExistsNotCreated);
                return this.LocalRedirect(returnUrl);
            }

            var result = await this.modelsService.CreateAsync(carModel);

            if (result)
            {
                this.Success(NotificationMessages.ModelCreated);
            }
            else
            {
                this.Error(NotificationMessages.ModelCreateError);

                model.Manufacturers = await this.GetAllManufacturers();
                return this.View(model);
            }

            return this.LocalRedirect(returnUrl);
        }

        private async Task<IEnumerable<SelectListItem>> GetAllManufacturers()
        {
            var manufacturers = (await this.manufacturersService
                    .GetAllAsync())
                .Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.Name
                });

            return manufacturers;
        }
    }
}