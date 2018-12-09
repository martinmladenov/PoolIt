namespace PoolIt.Web.Areas.Rides.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.JoinRequest;
    using Services.Contracts;
    using Services.Models;
    using Web.Controllers;

    [Authorize]
    [Area("Rides")]
    public class JoinRequestsController : BaseController
    {
        private readonly IRidesService ridesService;

        private readonly IJoinRequestsService joinRequestsService;

        public JoinRequestsController(IRidesService ridesService, IJoinRequestsService joinRequestsService)
        {
            this.ridesService = ridesService;
            this.joinRequestsService = joinRequestsService;
        }

        [Route("/ride/{id}/join")]
        public async Task<IActionResult> Create(string id)
        {
            var rideServiceModel = await this.ridesService.GetAsync(id);

            if (rideServiceModel == null
                || !this.ridesService.CanUserAccessRide(rideServiceModel, this.User?.Identity?.Name))
            {
                return this.NotFound();
            }

            if (!this.joinRequestsService.CanUserSendJoinRequest(rideServiceModel, this.User?.Identity?.Name))
            {
                return this.RedirectToAction("Details", "Rides", new {id});
            }

            var bindingModel = new JoinRequestCreateBindingModel
            {
                RideId = rideServiceModel.Id,
                RideTitle = rideServiceModel.Title
            };

            return this.View(bindingModel);
        }

        [HttpPost]
        [Route("/ride/{id}/join")]
        public async Task<IActionResult> Create(string id, JoinRequestCreateBindingModel model)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var rideServiceModel = await this.ridesService.GetAsync(id);

            if (rideServiceModel == null
                || !this.ridesService.CanUserAccessRide(rideServiceModel, this.User?.Identity?.Name))
            {
                return this.NotFound();
            }

            if (!this.joinRequestsService.CanUserSendJoinRequest(rideServiceModel, this.User?.Identity?.Name))
            {
                return this.RedirectToAction("Details", "Rides", new {id});
            }

            if (!this.ModelState.IsValid)
            {
                model.RideId = rideServiceModel.Id;
                model.RideTitle = rideServiceModel.Title;

                return this.View(model);
            }

            var serviceModel = Mapper.Map<JoinRequestServiceModel>(model);

            serviceModel.User = new PoolItUserServiceModel
            {
                UserName = this.User.Identity.Name
            };

            serviceModel.RideId = id;
            serviceModel.SentOn = DateTime.Now;

            await this.joinRequestsService.CreateAsync(serviceModel);

            return this.RedirectToAction("Details", "Rides", new {id});
        }

        [Route("/requests")]
        public async Task<IActionResult> Index()
        {
            var requests = await this.joinRequestsService.GetReceivedForUserAsync(this.User.Identity.Name);

            var viewModels = requests.Select(Mapper.Map<JoinRequestListingViewModel>)
                .OrderBy(r => r.RideId)
                .ToArray();

            return this.View(viewModels);
        }

        [HttpPost]
        public async Task<IActionResult> Accept(string id)
        {
            if (await this.joinRequestsService.CanUserAccessRequestAsync(id, this.User?.Identity?.Name))
            {
                await this.joinRequestsService.AcceptAsync(id);
            }

            return this.RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Refuse(string id)
        {
            if (await this.joinRequestsService.CanUserAccessRequestAsync(id, this.User?.Identity?.Name))
            {
                await this.joinRequestsService.DeleteAsync(id);
            }

            return this.RedirectToAction("Index");
        }
    }
}