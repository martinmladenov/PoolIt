namespace PoolIt.Services
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using PoolIt.Models;

    public class InvitationsService : DataService, IInvitationsService
    {
        private readonly IRandomStringGeneratorService generatorService;

        public InvitationsService(PoolItDbContext context, IRandomStringGeneratorService generatorService) :
            base(context)
        {
            this.generatorService = generatorService;
        }

        public async Task<string> GenerateAsync(string rideId)
        {
            if (rideId == null || !await this.context.Rides.AnyAsync(r => r.Id == rideId))
            {
                return null;
            }

            string generatedKey;

            do
            {
                generatedKey = this.generatorService.GenerateRandomString(6);
            } while (await this.context.Invitations.AnyAsync(r => r.Key == generatedKey));

            var invitation = new Invitation
            {
                RideId = rideId,
                Key = generatedKey
            };

            await this.context.Invitations.AddAsync(invitation);

            await this.context.SaveChangesAsync();

            return generatedKey;
        }

        public async Task<InvitationServiceModel> GetAsync(string key)
        {
            if (key == null)
            {
                return null;
            }

            var invitation = await this.context.Invitations
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

            var invitation = await this.context.Invitations
                .Include(i => i.Ride.JoinRequests)
                .SingleOrDefaultAsync(i => i.Key == invitationKey);

            var user = await this.context.Users.SingleOrDefaultAsync(u => u.UserName == userName);

            if (invitation == null || user == null)
            {
                return false;
            }

            var requests = invitation.Ride.JoinRequests
                .Where(r => r.UserId == user.Id)
                .ToArray();

            if (requests.Any())
            {
                this.context.JoinRequests.RemoveRange(requests);
            }

            var userRide = new UserRide
            {
                UserId = user.Id,
                RideId = invitation.RideId,
            };

            await this.context.UserRides.AddAsync(userRide);

            this.context.Invitations.Remove(invitation);

            await this.context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(string key)
        {
            if (key == null)
            {
                return false;
            }

            var invitation = await this.context.Invitations.SingleOrDefaultAsync(r => r.Key == key);

            if (invitation == null)
            {
                return false;
            }

            this.context.Invitations.Remove(invitation);

            await this.context.SaveChangesAsync();

            return true;
        }
    }
}