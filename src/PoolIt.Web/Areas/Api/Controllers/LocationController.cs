namespace PoolIt.Web.Areas.Api.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Services.Contracts;

    public class LocationController : ApiController
    {
        private readonly ILocationService locationService;

        public LocationController(ILocationService locationService)
        {
            this.locationService = locationService;
        }

        public async Task<IActionResult> GetTownName(double latitude, double longitude)
        {
            var townName = await this.locationService.GetTownNameAsync(latitude, longitude);

            return new JsonResult(new
            {
                townName
            });
        }
    }
}