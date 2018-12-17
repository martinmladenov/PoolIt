namespace PoolIt.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Contracts;
    using Services.Models;

    public class ModelsController : AdministrationController
    {
        public const string ManufacturerIdKey = "ManufacturerId";
        public const string ManufacturerNameKey = "ManufacturerName";
        public const string ModelsKey = "Models";

        private readonly IManufacturersService manufacturersService;
        private readonly IModelsService modelsService;

        public ModelsController(IManufacturersService manufacturersService, IModelsService modelsService)
        {
            this.manufacturersService = manufacturersService;
            this.modelsService = modelsService;
        }

        public async Task<IActionResult> Manufacturer(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var serviceModel = await this.manufacturersService.GetAsync(id);

            if (serviceModel == null)
            {
                return this.NotFound();
            }

            this.ViewData[ManufacturerIdKey] = serviceModel.Id;
            this.ViewData[ManufacturerNameKey] = serviceModel.Name;
            this.ViewData[ModelsKey] = serviceModel.Models.Select(Mapper.Map<CarModelAdminViewModel>);

            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CarModelAdminBindingModel model, string manufacturerId)
        {
            if (!this.ModelState.IsValid)
            {
                this.Error(NotificationMessages.ModelInvalidName);
                return this.RedirectToAction("Manufacturer", new {id = manufacturerId});
            }

            var serviceModel = Mapper.Map<CarModelServiceModel>(model);
            serviceModel.ManufacturerId = manufacturerId;

            if (await this.modelsService.ExistsAsync(serviceModel))
            {
                this.Error(NotificationMessages.ModelExists);
                return this.RedirectToAction("Manufacturer", new {id = manufacturerId});
            }

            var result = await this.modelsService.CreateAsync(serviceModel);

            if (result)
            {
                this.Success(NotificationMessages.ModelCreated);
            }
            else
            {
                this.Error(NotificationMessages.ModelCreateError);
            }

            return this.RedirectToAction("Manufacturer", new {id = manufacturerId});
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id, string manufacturerId)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var result = await this.modelsService.DeleteAsync(id);
            if (result)
            {
                this.Success(NotificationMessages.ModelDeleted);
            }
            else
            {
                this.Error(NotificationMessages.ModelDeleteError);
            }

            return this.RedirectToAction("Manufacturer", new {id = manufacturerId});
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CarModelAdminBindingModel model, string id, string manufacturerId)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var serviceModel = Mapper.Map<CarModelServiceModel>(model);
            serviceModel.Id = id;
            serviceModel.ManufacturerId = manufacturerId;

            if (await this.modelsService.ExistsAsync(serviceModel))
            {
                this.Error(NotificationMessages.ModelExists);
                return this.RedirectToAction("Manufacturer", new {id = manufacturerId});
            }

            var result = await this.modelsService.UpdateAsync(serviceModel);
            if (result)
            {
                this.Success(NotificationMessages.ModelEdited);
            }
            else
            {
                this.Error(NotificationMessages.ModelEditError);
            }

            return this.RedirectToAction("Manufacturer", new {id = manufacturerId});
        }
    }
}