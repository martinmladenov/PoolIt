namespace PoolIt.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper.QueryableExtensions;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using PoolIt.Models;
    using Services.Contracts;

    public class UsersController : AdministrationController
    {
        public const string SeniorAdminRoleName = "Senior Admin";
        public const string AdminRoleName = "Admin";
        public const string UserRoleName = "User";

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

            var seniorAdminIds = (await this.userManager
                    .GetUsersInRoleAsync(GlobalConstants.SeniorAdminRoleName))
                .Select(r => r.Id)
                .ToHashSet();

            var adminIds = (await this.userManager
                    .GetUsersInRoleAsync(GlobalConstants.AdminRoleName))
                .Select(r => r.Id)
                .ToHashSet();

            foreach (var user in users)
            {
                if (seniorAdminIds.Contains(user.Id))
                {
                    user.Role = SeniorAdminRoleName;
                }
                else if (adminIds.Contains(user.Id))
                {
                    user.Role = AdminRoleName;
                }
                else
                {
                    user.Role = UserRoleName;
                }
            }

            return this.View(users);
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.SeniorAdminRoleName)]
        public async Task<IActionResult> SetRole(string id, string role)
        {
            if (id == null || role == null)
            {
                return this.NotFound();
            }

            var user = await this.userManager.FindByIdAsync(id);

            if (user == null)
            {
                this.Error(NotificationMessages.UserRoleUpdateError);
                return this.RedirectToAction("Index");
            }

            switch (role)
            {
                case SeniorAdminRoleName:
                    await this.AddToRolesAsync(user, GlobalConstants.SeniorAdminRoleName,
                        GlobalConstants.AdminRoleName);
                    break;

                case AdminRoleName:
                    await this.RemoveFromRolesAsync(user, GlobalConstants.SeniorAdminRoleName);
                    await this.AddToRolesAsync(user, GlobalConstants.AdminRoleName);
                    break;

                case UserRoleName:
                    await this.RemoveFromRolesAsync(user, GlobalConstants.SeniorAdminRoleName,
                        GlobalConstants.AdminRoleName);
                    break;

                default:
                    this.Error(NotificationMessages.UserRoleUpdateError);
                    return this.RedirectToAction("Index");
            }

            this.Success(NotificationMessages.UserRoleUpdated);

            return this.RedirectToAction("Index");
        }

        private async Task AddToRolesAsync(PoolItUser user, params string[] roles)
        {
            foreach (var role in roles)
            {
                if (!await this.userManager.IsInRoleAsync(user, role))
                {
                    await this.userManager.AddToRoleAsync(user, role);
                }
            }
        }

        private async Task RemoveFromRolesAsync(PoolItUser user, params string[] roles)
        {
            foreach (var role in roles)
            {
                if (await this.userManager.IsInRoleAsync(user, role))
                {
                    await this.userManager.RemoveFromRoleAsync(user, role);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string id)
        {
            if (id == null)
            {
                return new JsonResult(new object());
            }

            var user = await this.userManager.FindByIdAsync(id);

            if (user == null ||
                !this.User.IsInRole(GlobalConstants.SeniorAdminRoleName) &&
                await this.userManager.IsInRoleAsync(user, GlobalConstants.AdminRoleName))
            {
                return this.BadRequest();
            }

            var newPassword = this.randomStringGeneratorHelper.GenerateRandomString(8);

            var token = await this.userManager.GeneratePasswordResetTokenAsync(user);

            var result = await this.userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
            {
                return this.BadRequest();
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
                                password != null &&
                                await this.userManager.CheckPasswordAsync(currentUser, password);

            if (!passwordValid)
            {
                this.Error(NotificationMessages.InvalidPassword);
                return this.RedirectToAction("Index");
            }

            var user = await this.userManager.FindByIdAsync(id);

            if (user == null ||
                !this.User.IsInRole(GlobalConstants.SeniorAdminRoleName) &&
                await this.userManager.IsInRoleAsync(user, GlobalConstants.AdminRoleName))
            {
                this.Error(NotificationMessages.AccountDeleteAdminError);
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