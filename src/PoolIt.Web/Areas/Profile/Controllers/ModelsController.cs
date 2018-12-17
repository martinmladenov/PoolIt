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
                Manufacturer = manufacturer,
                Model = model.CarModel
            };

            await this.modelsService.CreateAsync(carModel);

            returnUrl = returnUrl ?? "/";

            this.Success(NotificationMessages.ModelCreated);

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