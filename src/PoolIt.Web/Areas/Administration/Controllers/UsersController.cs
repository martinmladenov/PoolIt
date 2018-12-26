namespace PoolIt.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper.QueryableExtensions;
    using Infrastructure;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using PoolIt.Models;

    public class UsersController : AdministrationController
    {
        private readonly UserManager<PoolItUser> userManager;

        public UsersController(UserManager<PoolItUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = this.userManager.Users
                .ProjectTo<UserAdminListingModel>()
                .ToArray();

            var adminIds = (await this.userManager
                    .GetUsersInRoleAsync(GlobalConstants.AdminRoleName))
                .Select(r => r.Id)
                .ToHashSet();

            foreach (var user in users)
            {
                user.IsAdmin = adminIds.Contains(user.Id);
            }

            return this.View(users);
        }

        [HttpPost]
        public async Task<IActionResult> Promote(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var user = await this.userManager.FindByIdAsync(id);

            if (user == null || await this.userManager.IsInRoleAsync(user, GlobalConstants.AdminRoleName))
            {
                this.Error(NotificationMessages.UserPromoteError);
                return this.RedirectToAction("Index");
            }

            var result = await this.userManager.AddToRoleAsync(user, GlobalConstants.AdminRoleName);

            if (result.Succeeded)
            {
                this.Success(NotificationMessages.UserPromoted);
            }
            else
            {
                this.Error(NotificationMessages.UserPromoteError);
            }

            return this.RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Demote(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var user = await this.userManager.FindByIdAsync(id);

            if (user == null || !await this.userManager.IsInRoleAsync(user, GlobalConstants.AdminRoleName))
            {
                this.Error(NotificationMessages.UserDemoteError);
                return this.RedirectToAction("Index");
            }

            var result = await this.userManager.RemoveFromRoleAsync(user, GlobalConstants.AdminRoleName);

            if (result.Succeeded)
            {
                this.Success(NotificationMessages.UserDemoted);
            }
            else
            {
                this.Error(NotificationMessages.UserDemoteError);
            }

            return this.RedirectToAction("Index");
        }
    }
}