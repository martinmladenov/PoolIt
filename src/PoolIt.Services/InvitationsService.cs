namespace PoolIt.Services
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data.Common;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using PoolIt.Models;

    public class InvitationsService : BaseService, IInvitationsService
    {
        private readonly IRepository<Invitation> invitationsRepository;
        private readonly IRepository<Ride> ridesRepository;
        private readonly IRepository<JoinRequest> joinRequestsRepository;
        private readonly IRepository<PoolItUser> usersRepository;
        private readonly IRepository<UserRide> userRidesRepository;

        private readonly IRandomStringGeneratorService generatorService;

        public InvitationsService(IRepository<Invitation> invitationsRepository,
            IRepository<Ride> ridesRepository, IRepository<JoinRequest> joinRequestsRepository,
            IRepository<PoolItUser> usersRepository, IRepository<UserRide> userRidesRepository,
            IRandomStringGeneratorService generatorService)
        {
            this.invitationsRepository = invitationsRepository;
            this.ridesRepository = ridesRepository;
            this.joinRequestsRepository = joinRequestsRepository;
            this.usersRepository = usersRepository;
            this.userRidesRepository = userRidesRepository;
            this.generatorService = generatorService;
        }

        public async Task<string> GenerateAsync(string rideId)
        {
            if (rideId == null || !await this.ridesRepository.All().AnyAsync(r => r.Id == rideId))
            {
                return null;
            }

            string generatedKey;

            while (true)
            {
                generatedKey = this.generatorService.GenerateRandomString(length: 6);
                var key = generatedKey;
                if (!await this.invitationsRepository.All().AnyAsync(r => r.Key == key))
                {
                    break;
                }
            }

            var invitation = new Invitation
            {
                RideId = rideId,
                Key = generatedKey
            };

            await this.invitationsRepository.AddAsync(invitation);

            await this.invitationsRepository.SaveChangesAsync();

            return generatedKey;
        }

        public async Task<InvitationServiceModel> GetAsync(string key)
        {
            if (key == null)
            {
                return null;
            }

            var invitation = await this.invitationsRepository.All()
                .ProjectTo<InvitationServiceModel>()
                .SingleOrDefaultAsync(r => r.Key == key);

            return invitation;
        }

        public async Task<bool> AcceptAsync(string userName, string invitationKey)
        {
            if (userName == null || invitationKey == null)
            {
                return false;
            }

            var invitation = await this.invitationsRepository.All()
                .Include(i => i.Ride.JoinRequests)
                .Include(i => i.Ride.Participants)
                .ThenInclude(p => p.User)
                .SingleOrDefaultAsync(i => i.Key == invitationKey);

            var user = await this.usersRepository.All().SingleOrDefaultAsync(u => u.UserName == userName);

            if (invitation == null || user == null ||
                invitation.Ride.Participants.Any(p => p.User.UserName == userName))
            {
                return false;
            }

            var requests = invitation.Ride.JoinRequests
                .Where(r => r.UserId == user.Id)
                .ToArray();

            //Remove user's join requests for this ride
            foreach (var joinRequest in requests)
            {
                this.joinRequestsRepository.Remove(joinRequest);
            }

            var userRide = new UserRide
            {
                UserId = user.Id,
                RideId = invitation.RideId,
            };

            await this.userRidesRepository.AddAsync(userRide);

            //Delete invitation
            this.invitationsRepository.Remove(invitation);

            await this.invitationsRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(string key)
        {
            if (key == null)
            {
                return false;
            }

            var invitation = await this.invitationsRepository.All().SingleOrDefaultAsync(r => r.Key == key);

            if (invitation == null)
            {
                return false;
            }

            this.invitationsRepository.Remove(invitation);

            await this.invitationsRepository.SaveChangesAsync();

            return true;
        }
    }
}