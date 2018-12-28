namespace PoolIt.Web.Controllers
{
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using PoolIt.Models;
    using Services.Contracts;
    using Services.Models;

    public class ContactController : BaseController
    {
        private readonly IContactMessagesService contactMessagesService;
        private readonly UserManager<PoolItUser> userManager;

        public ContactController(IContactMessagesService contactMessagesService, UserManager<PoolItUser> userManager)
        {
            this.contactMessagesService = contactMessagesService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = this.User.Identity.IsAuthenticated
                ? await this.userManager.GetUserAsync(this.User)
                : null;

            if (user == null)
            {
                return this.View();
            }

            var model = new ContactMessageBindingModel
            {
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ContactMessageBindingModel model)
        {
            var user = this.User.Identity.IsAuthenticated
                ? await this.userManager.GetUserAsync(this.User)
                : null;

            if (!this.ModelState.IsValid)
            {
                model.FullName = $"{user.FirstName} {user.LastName}";
                model.Email = user.Email;

                return this.View(model);
            }

            var serviceModel = Mapper.Map<ContactMessageServiceModel>(model);

            serviceModel.UserId = user?.Id;

            var result = await this.contactMessagesService.CreateAsync(serviceModel);

            if (!result)
            {
                this.Error(NotificationMessages.ContactMessageCreateError);

                model.FullName = $"{user.FirstName} {user.LastName}";
                model.Email = user.Email;

                return this.View(model);
            }

            this.Success(NotificationMessages.ContactMessageCreated);
            return this.RedirectToAction("Index", "Home");
        }
    }
}