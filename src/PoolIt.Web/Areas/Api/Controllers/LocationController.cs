namespace PoolIt.Web.Areas.Api.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Services.Contracts;

    public class LocationController : ApiController
    {
        private readonly ILocationHelper locationHelper;

        public LocationController(ILocationHelper locationHelper)
        {
            this.locationHelper = locationHelper;
        }

        public async Task<IActionResult> GetTownName(double latitude, double longitude)
        {
            var townName = await this.locationHelper.GetTownNameAsync(latitude, longitude);

            return new JsonResult(new
            {
                townName
            });
        }
    }
}