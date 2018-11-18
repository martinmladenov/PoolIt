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

    public class CarsController : Controller
    {
        private readonly IManufacturersService manufacturersService;
        private readonly ICarsService carsService;

        public CarsController(IManufacturersService manufacturersService, ICarsService carsService)
        {
            this.manufacturersService = manufacturersService;
            this.carsService = carsService;
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            var manufacturers = await this.GetAllManufacturers();

            var model = new CarEditBindingModel
            {
                Manufacturers = manufacturers
            };
            
            return this.View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CarEditBindingModel model)
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
            
            await this.carsService.Create(serviceModel);
            
            return this.RedirectToAction("Index", "Home");
        }

        private async Task<IEnumerable<SelectListItem>> GetAllManufacturers()
        {
            var manufacturers = (await this.manufacturersService
                    .GetAll())
                .Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id
                });

            return manufacturers;
        }
    }
}