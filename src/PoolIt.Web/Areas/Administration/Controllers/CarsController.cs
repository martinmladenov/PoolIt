namespace PoolIt.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Contracts;

    public class CarsController : AdministrationController
    {
        private readonly ICarsService carsService;

        public CarsController(ICarsService carsService)
        {
            this.carsService = carsService;
        }

        public async Task<IActionResult> Index()
        {
            var cars = await this.carsService.GetAllAsync();

            return this.View(cars.Select(Mapper.Map<CarAdminListingViewModel>));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var result = await this.carsService.DeleteAsync(id);
            if (result)
            {
                this.Success(NotificationMessages.CarDeleted);
            }
            else
            {
                this.Error(NotificationMessages.CarDeleteError);
            }

            return this.RedirectToAction("Index");
        }
    }
}