namespace PoolIt.Web.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using PoolIt.Models;

    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<PoolItUser> userManager;

        public AccountController(UserManager<PoolItUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> ChangePassword()
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null)
            {
                return this.NotFound();
            }

            if (!await this.userManager.HasPasswordAsync(user))
            {
                return this.RedirectToAction("SetPassword");
            }

            var model = new UserChangePasswordBindingModel()
            {
                Email = user.Email
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserChangePasswordBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("ChangePassword");
            }

            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null)
            {
                return this.NotFound();
            }

            if (!await this.userManager.HasPasswordAsync(user))
            {
                return this.RedirectToAction("SetPassword");
            }

            var changePasswordResult =
                await this.userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }

                model.Email = user.Email;

                return this.View(model);
            }

            return this.RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> SetPassword()
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null)
            {
                return this.NotFound();
            }

            if (await this.userManager.HasPasswordAsync(user))
            {
                return this.RedirectToAction("ChangePassword");
            }

            var model = new UserSetPasswordBindingModel
            {
                Email = user.Email
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SetPassword(UserSetPasswordBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("SetPassword");
            }

            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null)
            {
                return this.NotFound();
            }

            if (await this.userManager.HasPasswordAsync(user))
            {
                return this.RedirectToAction("ChangePassword");
            }

            var changePasswordResult = await this.userManager.AddPasswordAsync(user, model.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }

                model.Email = user.Email;

                return this.View(model);
            }

            return this.RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Edit()
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null)
            {
                return this.NotFound();
            }

            var model = new UserEditBindingModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditBindingModel model)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null)
            {
                return this.NotFound();
            }

            if (!this.ModelState.IsValid)
            {
                model.Email = user.Email;

                return this.View(model);
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            await this.userManager.UpdateAsync(user);

            return this.RedirectToAction("Index", "Home");
        }
    }
}