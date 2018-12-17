namespace PoolIt.Web.Areas.Profile.Controllers
{
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CarModelCreateBindingModel model, string returnUrl)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
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
    }
}