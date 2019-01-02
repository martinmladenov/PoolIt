namespace PoolIt.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data;
    using Data.Common;
    using Data.Repository;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Newtonsoft.Json.Linq;
    using PoolIt.Models;
    using Xunit;

    public class PersonalDataServiceTests : BaseTests
    {
        [Fact]
        public async Task DeleteUser_WithCorrectUserId_WorksCorrectly()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var otherUsers = new List<PoolItUser>
            {
                new PoolItUser
                {
                    UserName = "otheruser0@example.com",
                    Email = "otheruser0@example.com",
                    FirstName = "Other0",
                    LastName = "User0"
                },
                new PoolItUser
                {
                    UserName = "otheruser1@example.com",
                    Email = "otheruser1@example.com",
                    FirstName = "Other1",
                    LastName = "User1"
                }
            };

            var ride = new Ride
            {
                Title = "Test Ride",
                Date = DateTime.UtcNow.AddDays(1),
                From = "Test From",
                To = "Test To",
                AvailableSeats = 3,
                PhoneNumber = "0123123123",
                Notes = null,
                Participants = new List<UserRide>
                {
                    new UserRide
                    {
                        User = otherUsers[0],
                    }
                },
                Invitations = new List<Invitation>
                {
                    new Invitation
                    {
                        Key = "testKey"
                    }
                },
                JoinRequests = new List<JoinRequest>
                {
                    new JoinRequest
                    {
                        User = otherUsers[1],
                        SentOn = DateTime.UtcNow.AddDays(-1),
                        Message = "Test Message"
                    }
                },
                Conversation = new Conversation
                {
                    Messages = new List<Message>
                    {
                        new Message
                        {
                            Author = otherUsers[0],
                            SentOn = DateTime.UtcNow.AddDays(-1),
                            Content = "Test Message Content 1"
                        },
                        new Message
                        {
                            Author = otherUsers[0],
                            SentOn = DateTime.UtcNow,
                            Content = "Test Message Content 2"
                        }
                    }
                }
            };

            var user = new PoolItUser
            {
                FirstName = "FirstName",
                LastName = "LastName",
                UserName = "testuser@example.com",
                Email = "testuser@example.com",
                SentRequests = new List<JoinRequest>
                {
                    new JoinRequest
                    {
                        Ride = new Ride
                        {
                            Title = "Test Ride for Join Request"
                        },
                        SentOn = DateTime.UtcNow,
                        Message = "Test Message for Join Request"
                    }
                },
                Cars = new List<Car>
                {
                    new Car
                    {
                        Model = new CarModel
                        {
                            Manufacturer = new CarManufacturer
                            {
                                Name = "TestManufacturer"
                            },
                            Model = "TestModel"
                        },
                        Colour = "TestColour",
                        Details = null,
                        Rides = new List<Ride>
                        {
                            ride
                        }
                    }
                },
                UserRides = new List<UserRide>
                {
                    new UserRide
                    {
                        Ride = ride
                    }
                },
                ContactMessages = new List<ContactMessage>
                {
                    new ContactMessage
                    {
                        Subject = "Test Subject",
                        Message = "Test Message"
                    }
                }
            };

            await context.Users.AddAsync(user);

            await context.SaveChangesAsync();

            var personalDataService = new PersonalDataService(new EfRepository<PoolItUser>(context),
                new EfRepository<JoinRequest>(context),
                new EfRepository<UserRide>(context),
                new EfRepository<Conversation>(context),
                new EfRepository<ContactMessage>(context));

            // Act
            var result = await personalDataService.DeleteUser(user.Id);

            // Assert
            Assert.True(result);

            var carCount = await context.Cars.CountAsync();
            Assert.Equal(0, carCount);

            var rideCount = await context.Rides.CountAsync();
            Assert.Equal(1, rideCount);

            var conversationCount = await context.Conversations.CountAsync();
            Assert.Equal(0, conversationCount);

            var invitationCount = await context.Invitations.CountAsync();
            Assert.Equal(0, invitationCount);

            var joinRequestCount = await context.JoinRequests.CountAsync();
            Assert.Equal(0, joinRequestCount);

            var messageCount = await context.Messages.CountAsync();
            Assert.Equal(0, messageCount);

            var userRideCount = await context.UserRides.CountAsync();
            Assert.Equal(0, userRideCount);

            var userCount = await context.Users.CountAsync();
            Assert.Equal(2, userCount);

            var contactMessageCount = await context.ContactMessages.CountAsync();
            Assert.Equal(0, contactMessageCount);
        }

        [Fact]
        public async Task DeleteUser_WithExceptionOnSave_ReturnsFalse()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var user = new PoolItUser
            {
                FirstName = "FirstName",
                LastName = "LastName",
                UserName = "testuser@example.com",
                Email = "testuser@example.com"
            };

            await context.Users.AddAsync(user);

            await context.SaveChangesAsync();

            var usersRepository = new EfRepository<PoolItUser>(context);

            var mockUsersRepository = new Mock<IRepository<PoolItUser>>();
            mockUsersRepository.Setup(r => r.All()).Returns(usersRepository.All());
            mockUsersRepository.Setup(r => r.SaveChangesAsync()).Throws<InvalidOperationException>();

            var personalDataService = new PersonalDataService(mockUsersRepository.Object,
                new EfRepository<JoinRequest>(context),
                new EfRepository<UserRide>(context),
                new EfRepository<Conversation>(context),
                new EfRepository<ContactMessage>(context));

            // Act
            var result = await personalDataService.DeleteUser(user.Id);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteUser_WithNonExistentUser_ReturnsFalse()
        {
            // Arrange
            var testId = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var personalDataService = new PersonalDataService(new EfRepository<PoolItUser>(context),
                new EfRepository<JoinRequest>(context),
                new EfRepository<UserRide>(context),
                new EfRepository<Conversation>(context),
                new EfRepository<ContactMessage>(context));

            // Act
            var result = await personalDataService.DeleteUser(testId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteUser_WithNullUserId_ReturnsFalse()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var personalDataService = new PersonalDataService(new EfRepository<PoolItUser>(context),
                new EfRepository<JoinRequest>(context),
                new EfRepository<UserRide>(context),
                new EfRepository<Conversation>(context),
                new EfRepository<ContactMessage>(context));

            // Act
            var result = await personalDataService.DeleteUser(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetPersonalDataForUserJson_WithCorrectUserId_WorksCorrectly()
        {
            // Arrange
            const string expectedJson =
                "{\"FirstName\": \"FirstName\",\"LastName\": \"LastName\",\"Email\": \"testuser@example.com\"," +
                "\"SentRequests\": [{\"Ride\": \"Test Ride for Join Request\",\"SentOn\": \"Thu, 01 Jan 2015 10:25:30 GMT\"," +
                "\"Message\": \"Test Message for Join Request\"}],\"Cars\": [{\"Manufacturer\": \"TestManufacturer\"," +
                "\"Model\": \"TestModel\",\"Colour\": \"TestColour\",\"Details\": null}],\"OrganisedRides\": " +
                "[{\"Title\": \"Test Ride\",\"Date\": \"Fri, 02 Jan 2015 10:25:30 GMT\",\"From\": \"Test From\"," +
                "\"To\": \"Test To\",\"AvailableSeats\": 3,\"PhoneNumber\": \"0123123123\",\"Notes\": null,\"Car\": " +
                "{\"Manufacturer\": \"TestManufacturer\",\"Model\": \"TestModel\",\"Colour\": \"TestColour\",\"Details\": null}," +
                "\"Participants\": [{\"FirstName\": \"Other0\",\"LastName\": \"User0\"},{\"FirstName\": \"FirstName\"," +
                "\"LastName\": \"LastName\"}],\"Invitations\": [{\"Key\": \"testKey\"}],\"JoinRequests\": " +
                "[{\"FirstName\": \"Other1\",\"LastName\": \"User1\",\"SentOn\": \"Wed, 31 Dec 2014 10:25:30 GMT\"," +
                "\"Message\": \"Test Message\"}]}],\"ParticipantInRides\": [{\"Title\": \"Test Ride\"," +
                "\"Date\": \"Fri, 02 Jan 2015 10:25:30 GMT\",\"From\": \"Test From\",\"To\": \"Test To\",\"Organiser\": " +
                "{\"FirstName\": \"FirstName\",\"LastName\": \"LastName\"},\"Car\": {\"Manufacturer\": \"TestManufacturer\"," +
                "\"Model\": \"TestModel\"},\"Participants\": [{\"FirstName\": \"Other0\",\"LastName\": \"User0\"}," +
                "{\"FirstName\": \"FirstName\",\"LastName\": \"LastName\"}],\"Conversation\": {\"Messages\": [{\"Author\": " +
                "{\"FirstName\": \"Other0\",\"LastName\": \"User0\"},\"SentOn\": \"Wed, 31 Dec 2014 10:25:30 GMT\"," +
                "\"Content\": \"Test Message Content 1\"},{\"Author\": {\"FirstName\": \"Other0\",\"LastName\": \"User0\"}," +
                "\"SentOn\": \"Thu, 01 Jan 2015 10:25:30 GMT\",\"Content\": \"Test Message Content 2\"}]}}]," +
                "\"ContactMessages\": [{\"Subject\": \"Test Subject\",\"Message\": \"Test Message\"}]}";

            var dateTime = new DateTime(2015, 1, 1, 10, 25, 30);

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var otherUsers = new List<PoolItUser>
            {
                new PoolItUser
                {
                    UserName = "otheruser0@example.com",
                    Email = "otheruser0@example.com",
                    FirstName = "Other0",
                    LastName = "User0"
                },
                new PoolItUser
                {
                    UserName = "otheruser1@example.com",
                    Email = "otheruser1@example.com",
                    FirstName = "Other1",
                    LastName = "User1"
                }
            };

            var ride = new Ride
            {
                Title = "Test Ride",
                Date = dateTime.AddDays(1),
                From = "Test From",
                To = "Test To",
                AvailableSeats = 3,
                PhoneNumber = "0123123123",
                Notes = null,
                Participants = new List<UserRide>
                {
                    new UserRide
                    {
                        User = otherUsers[0],
                    }
                },
                Invitations = new List<Invitation>
                {
                    new Invitation
                    {
                        Key = "testKey"
                    }
                },
                JoinRequests = new List<JoinRequest>
                {
                    new JoinRequest
                    {
                        User = otherUsers[1],
                        SentOn = dateTime.AddDays(-1),
                        Message = "Test Message"
                    }
                },
                Conversation = new Conversation
                {
                    Messages = new List<Message>
                    {
                        new Message
                        {
                            Author = otherUsers[0],
                            SentOn = dateTime.AddDays(-1),
                            Content = "Test Message Content 1"
                        },
                        new Message
                        {
                            Author = otherUsers[0],
                            SentOn = dateTime,
                            Content = "Test Message Content 2"
                        }
                    }
                }
            };

            var user = new PoolItUser
            {
                FirstName = "FirstName",
                LastName = "LastName",
                UserName = "testuser@example.com",
                Email = "testuser@example.com",
                SentRequests = new List<JoinRequest>
                {
                    new JoinRequest
                    {
                        Ride = new Ride
                        {
                            Title = "Test Ride for Join Request"
                        },
                        SentOn = dateTime,
                        Message = "Test Message for Join Request"
                    }
                },
                Cars = new List<Car>
                {
                    new Car
                    {
                        Model = new CarModel
                        {
                            Manufacturer = new CarManufacturer
                            {
                                Name = "TestManufacturer"
                            },
                            Model = "TestModel"
                        },
                        Colour = "TestColour",
                        Details = null,
                        Rides = new List<Ride>
                        {
                            ride
                        }
                    }
                },
                UserRides = new List<UserRide>
                {
                    new UserRide
                    {
                        Ride = ride
                    }
                },
                ContactMessages = new List<ContactMessage>
                {
                    new ContactMessage
                    {
                        Subject = "Test Subject",
                        Message = "Test Message"
                    }
                }
            };

            await context.Users.AddAsync(user);

            await context.SaveChangesAsync();

            var personalDataService =
                new PersonalDataService(new EfRepository<PoolItUser>(context), null, null, null, null);

            // Act
            var actualJson = await personalDataService.GetPersonalDataForUserJson(user.Id);

            // Assert
            var expectedResult = JToken.Parse(expectedJson);
            var actualResult = JToken.Parse(actualJson);

            var equal = JToken.DeepEquals(expectedResult, actualResult);

            Assert.True(equal);
        }

        [Fact]
        public async Task GetPersonalDataForUserJson_WithNonExistentUser_ReturnsNull()
        {
            // Arrange
            var testId = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var personalDataService =
                new PersonalDataService(new EfRepository<PoolItUser>(context), null, null, null, null);

            // Act
            var result = await personalDataService.GetPersonalDataForUserJson(testId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetPersonalDataForUserJson_WithNullUserId_ReturnsNull()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var personalDataService =
                new PersonalDataService(new EfRepository<PoolItUser>(context), null, null, null, null);

            // Act
            var result = await personalDataService.GetPersonalDataForUserJson(null);

            // Assert
            Assert.Null(result);
        }
    }
}