namespace PoolIt.Web.Areas.Api.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Services.Contracts;

    [Authorize]
    public class ModelsController : ApiController
    {
        private readonly IModelsService modelsService;

        public ModelsController(IModelsService modelsService)
        {
            this.modelsService = modelsService;
        }

        public async Task<IActionResult> GetByManufacturerId(string manufacturerId)
        {
            if (manufacturerId == null)
            {
                return new JsonResult(new object());
            }

            var models = (await this.modelsService
                    .GetAllByManufacturerAsync(manufacturerId))
                .Select(m => new
                {
                    m.Id,
                    m.Model
                });

            return new JsonResult(models);
        }
    }
}