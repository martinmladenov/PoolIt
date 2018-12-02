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

    [Authorize]
    [Area("Rides")]
    public class JoinRequestsController : Controller
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
            var rideServiceModel = await this.GetRide(id);

            if (rideServiceModel == null)
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
            if (!this.ModelState.IsValid)
            {
                var rideServiceModel = await this.GetRide(id);

                if (rideServiceModel == null)
                {
                    return this.RedirectToAction("Details", "Rides", new {id});
                }

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

        private async Task<RideServiceModel> GetRide(string id)
        {
            var rideServiceModel = await this.ridesService.GetAsync(id);

            if (rideServiceModel == null
                || rideServiceModel.Date < DateTime.Now
                || rideServiceModel.Participants.Any(r => r.User.Email == this.User.Identity.Name)
                || rideServiceModel.JoinRequests.Any(r => r.User.Email == this.User.Identity.Name)
                || rideServiceModel.Participants.Count >= rideServiceModel.AvailableSeats + 1)
            {
                return null;
            }

            return rideServiceModel;
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
            if (await this.CanUserAccessRequest(id))
            {
                await this.joinRequestsService.AcceptAsync(id);
            }

            return this.RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Refuse(string id)
        {
            if (await this.CanUserAccessRequest(id))
            {
                await this.joinRequestsService.DeleteAsync(id);
            }

            return this.RedirectToAction("Index");
        }

        private async Task<bool> CanUserAccessRequest(string id)
        {
            var request = await this.joinRequestsService.GetAsync(id);

            return request != null && request.Ride.Car.Owner.UserName == this.User.Identity.Name;
        }
    }
}