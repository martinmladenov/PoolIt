namespace PoolIt.Services
{
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Data.Common;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using PoolIt.Models;

    public class PersonalDataService : IPersonalDataService
    {
        private readonly IRepository<PoolItUser> usersRepository;
        private readonly IRepository<JoinRequest> joinRequestsRepository;
        private readonly IRepository<UserRide> userRidesRepository;
        private readonly IRepository<Conversation> conversationsRepository;

        public PersonalDataService(IRepository<PoolItUser> usersRepository,
            IRepository<JoinRequest> joinRequestsRepository, IRepository<UserRide> userRidesRepository,
            IRepository<Conversation> conversationsRepository)
        {
            this.usersRepository = usersRepository;
            this.joinRequestsRepository = joinRequestsRepository;
            this.userRidesRepository = userRidesRepository;
            this.conversationsRepository = conversationsRepository;
        }

        public async Task<string> GetPersonalDataForUserJson(string userId)
        {
            if (userId == null)
            {
                return null;
            }

            var user = await this.usersRepository.All()
                .Include(u => u.Cars).ThenInclude(c => c.Model).ThenInclude(c => c.Manufacturer)
                .Include(u => u.Cars).ThenInclude(c => c.Rides).ThenInclude(c => c.Invitations)
                .Include(u => u.Cars).ThenInclude(c => c.Rides).ThenInclude(c => c.Participants)
                .ThenInclude(c => c.User)
                .Include(u => u.Cars).ThenInclude(c => c.Rides).ThenInclude(c => c.JoinRequests)
                .ThenInclude(c => c.User)
                .Include(u => u.Cars).ThenInclude(c => c.Rides).ThenInclude(c => c.Conversation.Messages)
                .ThenInclude(c => c.Author)
                .Include(u => u.SentRequests).ThenInclude(r => r.Ride)
                .Include(u => u.UserRides).ThenInclude(r => r.Ride.Car.Owner)
                .Include(u => u.UserRides).ThenInclude(r => r.Ride.Car.Model.Manufacturer)
                .Include(u => u.UserRides).ThenInclude(r => r.Ride.Conversation.Messages).ThenInclude(m => m.Author)
                .Include(u => u.UserRides).ThenInclude(r => r.Ride.Participants).ThenInclude(p => p.User)
                .SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            var personalData = new
            {
                user.FirstName,
                user.LastName,
                user.Email,
                SentRequests = user.SentRequests.Select(j => new
                    {
                        Ride = j.Ride.Title,
                        SentOn = j.SentOn.ToString("R"),
                        j.Message
                    })
                    .ToArray(),
                Cars = user.Cars.Select(c => new
                    {
                        Manufacturer = c.Model.Manufacturer.Name,
                        c.Model.Model,
                        c.Colour,
                        c.Details
                    })
                    .OrderBy(c => c.Manufacturer)
                    .ThenBy(c => c.Model)
                    .ToArray(),
                OrganisedRides = user.Cars
                    .SelectMany(c => c.Rides)
                    .OrderBy(r => r.Date)
                    .Select(r => new
                    {
                        r.Title,
                        Date = r.Date.ToString("R"),
                        r.From,
                        r.To,
                        r.AvailableSeats,
                        r.PhoneNumber,
                        r.Notes,
                        Car = new
                        {
                            Manufacturer = r.Car.Model.Manufacturer.Name,
                            r.Car.Model.Model,
                            r.Car.Colour,
                            r.Car.Details
                        },
                        Participants = r.Participants.Select(p => new
                            {
                                p.User.FirstName,
                                p.User.LastName
                            })
                            .ToArray(),
                        Invitations = r.Invitations.Select(i => new
                            {
                                i.Key
                            })
                            .ToArray(),
                        JoinRequests = r.JoinRequests.Select(j => new
                            {
                                j.User.FirstName,
                                j.User.LastName,
                                SentOn = j.SentOn.ToString("R"),
                                j.Message
                            })
                            .ToArray()
                    })
                    .ToArray(),
                ParticipantInRides = user.UserRides
                    .Select(ur => ur.Ride)
                    .OrderBy(r => r.Date)
                    .Select(r => new
                    {
                        r.Title,
                        Date = r.Date.ToString("R"),
                        r.From,
                        r.To,
                        Organiser = new
                        {
                            r.Car.Owner.FirstName,
                            r.Car.Owner.LastName
                        },
                        Car = new
                        {
                            Manufacturer = r.Car.Model.Manufacturer.Name,
                            r.Car.Model.Model
                        },
                        Participants = r.Participants.Select(p => new
                            {
                                p.User.FirstName,
                                p.User.LastName
                            })
                            .ToArray(),
                        Conversation = new
                        {
                            Messages = r.Conversation.Messages
                                .OrderBy(m => m.SentOn)
                                .Select(m => new
                                {
                                    Author = new
                                    {
                                        m.Author.FirstName,
                                        m.Author.LastName
                                    },
                                    SentOn = m.SentOn.ToString("R"),
                                    m.Content
                                })
                                .ToArray()
                        }
                    })
                    .ToArray()
            };

            var json = JsonConvert.SerializeObject(personalData, Formatting.Indented);

            return json;
        }

        public async Task<bool> DeleteUser(string userId)
        {
            if (userId == null)
            {
                return false;
            }

            var user = await this.usersRepository.All()
                .SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return false;
            }

            try
            {
                var joinRequests = this.joinRequestsRepository.All()
                    .Where(r => r.UserId == userId)
                    .ToArray();

                this.joinRequestsRepository.RemoveRange(joinRequests);

                var userRides = this.userRidesRepository.All()
                    .Where(r => r.UserId == userId)
                    .ToArray();

                this.userRidesRepository.RemoveRange(userRides);

                var conversations = this.conversationsRepository.All()
                    .Where(c => c.Ride.Car.OwnerId == userId)
                    .ToArray();

                this.usersRepository.Remove(user);

                await this.usersRepository.SaveChangesAsync();

                this.conversationsRepository.RemoveRange(conversations);

                await this.conversationsRepository.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}