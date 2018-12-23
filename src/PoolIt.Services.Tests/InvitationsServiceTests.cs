namespace PoolIt.Services.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Data;
    using Data.Repository;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using PoolIt.Models;
    using Xunit;

    public class InvitationsServiceTests : BaseTests
    {
        #region GenerateAsync_Tests

        [Fact]
        public async Task GenerateAsync_WithCorrectRideId_WorksCorrectly()
        {
            // Arrange
            const string expectedKey = "expkey";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride
            {
                Title = "TestRide"
            };

            await context.Rides.AddAsync(ride);

            await context.SaveChangesAsync();

            var mockRandomStringGeneratorHelper = new Mock<IRandomStringGeneratorHelper>();

            mockRandomStringGeneratorHelper.Setup(r => r.GenerateRandomString(It.IsAny<int>())).Returns(expectedKey);

            var invitationsService = new InvitationsService(new EfRepository<Invitation>(context), new EfRepository<Ride>(context),
                null, null, null, mockRandomStringGeneratorHelper.Object);

            // Act
            var resultKey = await invitationsService.GenerateAsync(ride.Id);

            // Assert
            Assert.Equal(expectedKey, resultKey);

            var invitationExists = await context.Invitations.AnyAsync(i => i.Key == resultKey);
            Assert.True(invitationExists);
        }

        [Fact]
        public async Task GenerateAsync_WithExistingKeyGenerated_WorksCorrectly()
        {
            // Arrange
            const string existingKey = "exikey";
            const string newKey = "newkey";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride
            {
                Title = "TestRide"
            };

            await context.Rides.AddAsync(ride);

            await context.Invitations.AddAsync(new Invitation
            {
                Key = existingKey
            });

            await context.SaveChangesAsync();

            var mockRandomStringGeneratorHelper = new Mock<IRandomStringGeneratorHelper>();

            mockRandomStringGeneratorHelper.SetupSequence(r => r.GenerateRandomString(It.IsAny<int>()))
                .Returns(existingKey)
                .Returns(newKey);

            var invitationsService = new InvitationsService(new EfRepository<Invitation>(context), new EfRepository<Ride>(context),
                null, null, null, mockRandomStringGeneratorHelper.Object);

            // Act
            var resultKey = await invitationsService.GenerateAsync(ride.Id);

            // Assert
            Assert.Equal(newKey, resultKey);

            mockRandomStringGeneratorHelper.Verify(r => r.GenerateRandomString(It.IsAny<int>()), Times.Exactly(2));

            var invitationExists = await context.Invitations.AnyAsync(i => i.Key == resultKey);
            Assert.True(invitationExists);
        }

        [Fact]
        public async Task GenerateAsync_WithNullRideId_ReturnsNull()
        {
            // Arrange

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var mockRandomStringGeneratorHelper = new Mock<IRandomStringGeneratorHelper>();

            mockRandomStringGeneratorHelper.Setup(r => r.GenerateRandomString(It.IsAny<int>())).Returns("abcdef");

            var invitationsService = new InvitationsService(new EfRepository<Invitation>(context), new EfRepository<Ride>(context),
                null, null, null, mockRandomStringGeneratorHelper.Object);

            // Act
            var resultKey = await invitationsService.GenerateAsync(null);

            // Assert
            Assert.Null(resultKey);

            var invitationCount = await context.Invitations.CountAsync();
            Assert.Equal(0, invitationCount);
        }

        [Fact]
        public async Task GenerateAsync_WithNonExistentRide_ReturnsNull()
        {
            // Arrange
            var testRideId = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var mockRandomStringGeneratorHelper = new Mock<IRandomStringGeneratorHelper>();

            mockRandomStringGeneratorHelper.Setup(r => r.GenerateRandomString(It.IsAny<int>())).Returns("abcdef");

            var invitationsService = new InvitationsService(new EfRepository<Invitation>(context), new EfRepository<Ride>(context),
                null, null, null, mockRandomStringGeneratorHelper.Object);

            // Act
            var resultKey = await invitationsService.GenerateAsync(testRideId);

            // Assert
            Assert.Null(resultKey);

            var invitationCount = await context.Invitations.CountAsync();
            Assert.Equal(0, invitationCount);
        }

        #endregion

        #region GetAsync_Tests

        [Fact]
        public async Task GetAsync_WithCorrectKey_WorksCorrectly()
        {
            // Arrange
            const string testKey = "key111";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride
            {
                Car = new Car
                {
                    Model = new CarModel
                    {
                        Manufacturer = new CarManufacturer()
                    },
                    Owner = new PoolItUser()
                },
                Conversation = new Conversation()
            };

            await context.Invitations.AddRangeAsync(
                new Invitation
                {
                    Key = testKey,
                    Ride = ride
                },
                new Invitation
                {
                    Key = "key002",
                    Ride = ride
                },
                new Invitation
                {
                    Key = "key003",
                    Ride = ride
                }
            );

            context.SaveChanges();

            var invitationsService = new InvitationsService(new EfRepository<Invitation>(context),
                null, null, null, null, null);

            // Act
            var result = await invitationsService.GetAsync(testKey);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAsync_WithNullKey_ReturnsNull()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride
            {
                Car = new Car
                {
                    Model = new CarModel
                    {
                        Manufacturer = new CarManufacturer()
                    },
                    Owner = new PoolItUser()
                },
                Conversation = new Conversation()
            };

            await context.Invitations.AddRangeAsync(
                new Invitation
                {
                    Key = "key002",
                    Ride = ride
                },
                new Invitation
                {
                    Key = "key003",
                    Ride = ride
                }
            );

            context.SaveChanges();

            var invitationsService = new InvitationsService(new EfRepository<Invitation>(context),
                null, null, null, null, null);

            // Act
            var result = await invitationsService.GetAsync(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_WithNonExistentInvitation_ReturnsNull()
        {
            // Arrange
            const string testKey = "key001";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride
            {
                Car = new Car
                {
                    Model = new CarModel
                    {
                        Manufacturer = new CarManufacturer()
                    },
                    Owner = new PoolItUser()
                },
                Conversation = new Conversation()
            };

            await context.Invitations.AddRangeAsync(
                new Invitation
                {
                    Key = "key002",
                    Ride = ride
                },
                new Invitation
                {
                    Key = "key003",
                    Ride = ride
                }
            );

            context.SaveChanges();

            var invitationsService = new InvitationsService(new EfRepository<Invitation>(context),
                null, null, null, null, null);

            // Act
            var result = await invitationsService.GetAsync(testKey);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region AcceptAsync_Tests

        [Fact]
        public async Task AcceptAsync_WithCorrectUserAndInvitation_WorksCorrectly()
        {
            // Arrange
            const string testKey = "key111";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride();

            await context.Rides.AddAsync(ride);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            await context.Users.AddAsync(user);

            var joinRequest = new JoinRequest
            {
                User = user,
                Ride = ride
            };

            await context.JoinRequests.AddAsync(joinRequest);

            await context.Invitations.AddAsync(new Invitation
            {
                Key = testKey,
                Ride = ride
            });

            context.SaveChanges();

            var invitationsService = new InvitationsService(
                new EfRepository<Invitation>(context),
                new EfRepository<Ride>(context),
                new EfRepository<JoinRequest>(context),
                new EfRepository<PoolItUser>(context),
                new EfRepository<UserRide>(context),
                null);

            // Act
            var result = await invitationsService.AcceptAsync(user.UserName, testKey);

            // Assert
            Assert.True(result);

            // - Check for UserRide creation
            var userRideExists = await context.UserRides.AnyAsync(u => u.UserId == user.Id && u.RideId == ride.Id);
            Assert.True(userRideExists);

            // - Check JoinRequest is removed
            var joinRequestExists = await context.JoinRequests.AnyAsync();
            Assert.False(joinRequestExists);

            // - Check invitation is removed
            var invitationExists = await context.Invitations.AnyAsync();
            Assert.False(invitationExists);
        }

        [Fact]
        public async Task AcceptAsync_WithNullUserName_ReturnsFalse()
        {
            // Arrange
            const string testKey = "key111";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride();

            await context.Rides.AddAsync(ride);

            await context.Invitations.AddAsync(new Invitation
            {
                Key = testKey,
                Ride = ride
            });

            context.SaveChanges();

            var invitationsService = new InvitationsService(
                new EfRepository<Invitation>(context),
                new EfRepository<Ride>(context),
                new EfRepository<JoinRequest>(context),
                new EfRepository<PoolItUser>(context),
                new EfRepository<UserRide>(context),
                null);

            // Act
            var result = await invitationsService.AcceptAsync(null, testKey);

            // Assert
            Assert.False(result);

            var invitationExists = await context.Invitations.AnyAsync();
            Assert.True(invitationExists);
        }

        [Fact]
        public async Task AcceptAsync_WithNullInvitationKey_ReturnsFalse()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            await context.Users.AddAsync(user);

            context.SaveChanges();

            var invitationsService = new InvitationsService(
                new EfRepository<Invitation>(context),
                new EfRepository<Ride>(context),
                new EfRepository<JoinRequest>(context),
                new EfRepository<PoolItUser>(context),
                new EfRepository<UserRide>(context),
                null);

            // Act
            var result = await invitationsService.AcceptAsync(user.UserName, null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AcceptAsync_WithNonExistentInvitation_ReturnsFalse()
        {
            // Arrange
            const string testKey = "key111";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride();

            await context.Rides.AddAsync(ride);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            await context.Users.AddAsync(user);

            var invitationsService = new InvitationsService(
                new EfRepository<Invitation>(context),
                new EfRepository<Ride>(context),
                new EfRepository<JoinRequest>(context),
                new EfRepository<PoolItUser>(context),
                new EfRepository<UserRide>(context),
                null);

            // Act
            var result = await invitationsService.AcceptAsync(user.UserName, testKey);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AcceptAsync_WithNonExistentUser_ReturnsFalse()
        {
            // Arrange
            const string testKey = "key111";
            const string testUserName = "TestUser";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride();

            await context.Rides.AddAsync(ride);

            await context.Invitations.AddAsync(new Invitation
            {
                Key = testKey,
                Ride = ride
            });

            context.SaveChanges();

            var invitationsService = new InvitationsService(
                new EfRepository<Invitation>(context),
                new EfRepository<Ride>(context),
                new EfRepository<JoinRequest>(context),
                new EfRepository<PoolItUser>(context),
                new EfRepository<UserRide>(context),
                null);

            // Act
            var result = await invitationsService.AcceptAsync(testUserName, testKey);

            // Assert
            Assert.False(result);

            var invitationExists = await context.Invitations.AnyAsync();
            Assert.True(invitationExists);
        }

        [Fact]
        public async Task AcceptAsync_WithUserAlreadyJoined_ReturnsFalse()
        {
            // Arrange
            const string testKey = "key111";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride();

            await context.Rides.AddAsync(ride);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            await context.Users.AddAsync(user);

            var userRide = new UserRide
            {
                User = user,
                Ride = ride
            };

            await context.UserRides.AddAsync(userRide);

            await context.Invitations.AddAsync(new Invitation
            {
                Key = testKey,
                Ride = ride
            });

            context.SaveChanges();

            var invitationsService = new InvitationsService(
                new EfRepository<Invitation>(context),
                new EfRepository<Ride>(context),
                new EfRepository<JoinRequest>(context),
                new EfRepository<PoolItUser>(context),
                new EfRepository<UserRide>(context),
                null);

            // Act
            var result = await invitationsService.AcceptAsync(user.UserName, testKey);

            // Assert
            Assert.False(result);

            var invitationExists = await context.Invitations.AnyAsync();
            Assert.True(invitationExists);
        }

        #endregion

        #region DeleteAsync_Tests

        [Fact]
        public async Task DeleteAsync_WithCorrectKey_WorksCorrectly()
        {
            const string testKey = "key111";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            await context.Invitations.AddRangeAsync(
                new Invitation
                {
                    Key = testKey
                },
                new Invitation
                {
                    Key = "key002"
                },
                new Invitation
                {
                    Key = "key003"
                });

            context.SaveChanges();

            var invitationsService = new InvitationsService(new EfRepository<Invitation>(context),
                null, null, null, null, null);

            // Act
            var result = await invitationsService.DeleteAsync(testKey);

            // Assert
            Assert.True(result);

            var invitationExists = await context.Invitations.AnyAsync(i => i.Key == testKey);
            Assert.False(invitationExists);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistentInvitation_ReturnsFalse()
        {
            const string testKey = "key111";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            await context.Invitations.AddRangeAsync(
                new Invitation
                {
                    Key = "key002"
                },
                new Invitation
                {
                    Key = "key003"
                });

            context.SaveChanges();

            var invitationsService = new InvitationsService(new EfRepository<Invitation>(context),
                null, null, null, null, null);

            // Act
            var result = await invitationsService.DeleteAsync(testKey);

            // Assert
            Assert.False(result);

            var invitationsCount = await context.Invitations.CountAsync();
            Assert.Equal(2, invitationsCount);
        }

        [Fact]
        public async Task DeleteAsync_WithNullKey_ReturnsFalse()
        {
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            await context.Invitations.AddRangeAsync(
                new Invitation
                {
                    Key = "key002"
                },
                new Invitation
                {
                    Key = "key003"
                });

            context.SaveChanges();

            var invitationsService = new InvitationsService(new EfRepository<Invitation>(context),
                null, null, null, null, null);

            // Act
            var result = await invitationsService.DeleteAsync(null);

            // Assert
            Assert.False(result);

            var invitationsCount = await context.Invitations.CountAsync();
            Assert.Equal(2, invitationsCount);
        }

        #endregion

        #region GetAllAsync_Tests

        [Fact]
        public async Task GetAllAsync_WithInvitations_WorksCorrectly()
        {
            const int expectedResult = 2;

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride
            {
                Car = new Car
                {
                    Model = new CarModel
                    {
                        Manufacturer = new CarManufacturer()
                    },
                    Owner = new PoolItUser()
                },
                Conversation = new Conversation()
            };

            await context.Invitations.AddRangeAsync(
                new Invitation
                {
                    Key = "key001",
                    Ride = ride
                },
                new Invitation
                {
                    Key = "key002",
                    Ride = ride
                });

            context.SaveChanges();

            var invitationsService = new InvitationsService(new EfRepository<Invitation>(context),
                null, null, null, null, null);

            // Act
            var actualResult = (await invitationsService.GetAllAsync()).Count();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async Task GetAllAsync_WithNoInvitations_ReturnsEmptyEnumerable()
        {
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var invitationsService = new InvitationsService(new EfRepository<Invitation>(context),
                null, null, null, null, null);

            // Act
            var actualResult = (await invitationsService.GetAllAsync()).Count();

            // Assert
            Assert.Equal(0, actualResult);
        }

        #endregion
    }
}
