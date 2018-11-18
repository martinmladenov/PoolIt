namespace PoolIt.Web.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Contracts;
    using Services.Models;

    public class ModelsController : Controller
    {
        private readonly IManufacturersService manufacturersService;
        private readonly IModelsService modelsService;

        public ModelsController(IManufacturersService manufacturersService, IModelsService modelsService)
        {
            this.manufacturersService = manufacturersService;
            this.modelsService = modelsService;
        }

        [Authorize]
        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CarModelCreateBindingModel model, string returnUrl)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            var manufacturer = await this.manufacturersService.Get(model.Manufacturer);

            if (manufacturer == null)
            {
                manufacturer = new CarManufacturerServiceModel
                {
                    Name = model.Manufacturer
                };
            }

            var carModel = new CarModelServiceModel
            {
                Manufacturer = manufacturer,
                Model = model.CarModel
            };

            await this.modelsService.Create(carModel);

            returnUrl = returnUrl ?? "/";

            return this.LocalRedirect(returnUrl);
        }
    }
}