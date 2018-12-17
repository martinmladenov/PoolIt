namespace PoolIt.Web.Areas.Administration.Controllers
{
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Web.Controllers;

    [Area("Administration")]
    [Authorize(Roles = GlobalConstants.AdminRoleName)]
    public abstract class AdministrationController : BaseController
    {
    }
}