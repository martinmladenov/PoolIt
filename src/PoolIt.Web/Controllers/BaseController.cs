namespace PoolIt.Web.Controllers
{
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;

    public abstract class BaseController : Controller
    {
        protected void Error(string message)
        {
            this.TempData[GlobalConstants.TempDataErrorMessageKey] = message;
        }

        protected void Success(string message)
        {
            this.TempData[GlobalConstants.TempDataSuccessMessageKey] = message;
        }
    }
}