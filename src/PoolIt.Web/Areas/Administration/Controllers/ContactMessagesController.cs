namespace PoolIt.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Contracts;

    public class ContactMessagesController : AdministrationController
    {
        private readonly IContactMessagesService contactMessagesService;

        public ContactMessagesController(IContactMessagesService contactMessagesService)
        {
            this.contactMessagesService = contactMessagesService;
        }

        public async Task<IActionResult> Index()
        {
            var messages = await this.contactMessagesService.GetAllAsync();

            return this.View(messages.Select(Mapper.Map<ContactMessageAdminListingViewModel>));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var result = await this.contactMessagesService.DeleteAsync(id);

            if (result)
            {
                this.Success(NotificationMessages.ContactMessageDeleted);
            }
            else
            {
                this.Error(NotificationMessages.ContactMessageDeleteError);
            }

            return this.RedirectToAction("Index");
        }
    }
}