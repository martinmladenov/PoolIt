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
    using Services.Contracts;

    public class UsersController : AdministrationController
    {
        private readonly UserManager<PoolItUser> userManager;
        private readonly IPersonalDataService personalDataService;
        private readonly IRandomStringGeneratorHelper randomStringGeneratorHelper;

        public UsersController(UserManager<PoolItUser> userManager, IPersonalDataService personalDataService,
            IRandomStringGeneratorHelper randomStringGeneratorHelper)
        {
            this.userManager = userManager;
            this.personalDataService = personalDataService;
            this.randomStringGeneratorHelper = randomStringGeneratorHelper;
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

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string id)
        {
            if (id == null)
            {
                return new JsonResult(new object());
            }

            var user = await this.userManager.FindByIdAsync(id);

            if (user == null)
            {
                return new JsonResult(new object());
            }

            var newPassword = this.randomStringGeneratorHelper.GenerateRandomString(8);

            var token = await this.userManager.GeneratePasswordResetTokenAsync(user);

            var result = await this.userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
            {
                return new JsonResult(new object());
            }

            return new JsonResult(new
            {
                name = $"{user.FirstName} {user.LastName}",
                newPassword
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(string password, string id)
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);

            var passwordValid = !await this.userManager.HasPasswordAsync(currentUser) ||
                                await this.userManager.CheckPasswordAsync(currentUser, password);

            if (!passwordValid)
            {
                this.Error(NotificationMessages.InvalidPassword);
                return this.RedirectToAction("Index");
            }

            var result = await this.personalDataService.DeleteUser(id);

            if (!result)
            {
                this.Error(NotificationMessages.AccountDeleteAdminError);
                return this.RedirectToAction("Index");
            }

            this.Success(NotificationMessages.AccountDeletedAdmin);
            return this.RedirectToAction("Index");
        }
    }
}