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

    public class ManufacturersController : AdministrationController
    {
        public const string ManufacturersKey = "Manufacturers";

        private readonly IManufacturersService manufacturersService;

        public ManufacturersController(IManufacturersService manufacturersService)
        {
            this.manufacturersService = manufacturersService;
        }

        public async Task<IActionResult> Index()
        {
            var manufacturers = await this.manufacturersService.GetAllAsync();

            this.ViewData[ManufacturersKey] = manufacturers.Select(Mapper.Map<CarManufacturerAdminViewModel>);

            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CarManufacturerAdminBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                this.Error(NotificationMessages.ManufacturerInvalidName);
                return this.RedirectToAction("Index");
            }

            var serviceModel = Mapper.Map<CarManufacturerServiceModel>(model);

            if (await this.manufacturersService.ExistsAsync(serviceModel))
            {
                this.Error(NotificationMessages.ManufacturerExists);
                return this.RedirectToAction("Index");
            }

            var result = await this.manufacturersService.CreateAsync(serviceModel);

            if (result)
            {
                this.Success(NotificationMessages.ManufacturerCreated);
            }
            else
            {
                this.Error(NotificationMessages.ManufacturerCreateError);
            }

            return this.RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var result = await this.manufacturersService.DeleteAsync(id);
            if (result)
            {
                this.Success(NotificationMessages.ManufacturerDeleted);
            }
            else
            {
                this.Error(NotificationMessages.ManufacturerDeleteError);
            }

            return this.RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CarManufacturerAdminBindingModel model, string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var serviceModel = Mapper.Map<CarManufacturerServiceModel>(model);

            if (await this.manufacturersService.ExistsAsync(serviceModel))
            {
                this.Error(NotificationMessages.ManufacturerExists);
                return this.RedirectToAction("Index");
            }

            serviceModel.Id = id;

            var result = await this.manufacturersService.UpdateAsync(serviceModel);
            if (result)
            {
                this.Success(NotificationMessages.ManufacturerEdited);
            }
            else
            {
                this.Error(NotificationMessages.ManufacturerEditError);
            }

            return this.RedirectToAction("Index");
        }
    }
}