namespace PoolIt.Services.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Data.Repository;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using PoolIt.Models;
    using Xunit;

    public class JoinRequestsServiceTests : BaseTests
    {
        #region CreateAsync_Tests

        [Fact]
        public async Task CreateAsync_WithValidModel_WorksCorrectly()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride
            {
                Title = "TestRide"
            };

            await context.Rides.AddAsync(ride);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            await context.Users.AddAsync(user);

            await context.SaveChangesAsync();

            var joinRequestsService = new JoinRequestsService(
                new EfRepository<JoinRequest>(context),
                new EfRepository<Ride>(context),
                new EfRepository<PoolItUser>(context),
                null);

            var serviceModel = new JoinRequestServiceModel
            {
                RideId = ride.Id,
                User = new PoolItUserServiceModel
                {
                    UserName = user.UserName
                },
                Message = "Test Message"
            };

            // Act
            var result = await joinRequestsService.CreateAsync(serviceModel);

            // Assert
            Assert.True(result);

            var dbModel = await context.JoinRequests.SingleOrDefaultAsync();
            Assert.NotNull(dbModel);

            Assert.Equal(user.Id, dbModel.UserId);
        }

        [Fact]
        public async Task CreateAsync_WithInvalidModel_ReturnsFalse()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride
            {
                Title = "TestRide"
            };

            await context.Rides.AddAsync(ride);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            await context.Users.AddAsync(user);

            await context.SaveChangesAsync();

            var joinRequestsService = new JoinRequestsService(
                new EfRepository<JoinRequest>(context),
                new EfRepository<Ride>(context),
                new EfRepository<PoolItUser>(context),
                null);

            var serviceModel = new JoinRequestServiceModel
            {
                RideId = ride.Id,
                User = new PoolItUserServiceModel
                {
                    UserName = user.UserName
                }
            };

            // Act
            var result = await joinRequestsService.CreateAsync(serviceModel);

            // Assert
            Assert.False(result);

            var existsInDb = await context.JoinRequests.AnyAsync();
            Assert.False(existsInDb);
        }

        [Fact]
        public async Task CreateAsync_WithNonExistentRide_ReturnsFalse()
        {
            // Arrange
            var testRideId = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            await context.Users.AddAsync(user);

            await context.SaveChangesAsync();

            var joinRequestsService = new JoinRequestsService(
                new EfRepository<JoinRequest>(context),
                new EfRepository<Ride>(context),
                new EfRepository<PoolItUser>(context),
                null);

            var serviceModel = new JoinRequestServiceModel
            {
                RideId = testRideId,
                User = new PoolItUserServiceModel
                {
                    UserName = user.UserName
                },
                Message = "Test Message"
            };

            // Act
            var result = await joinRequestsService.CreateAsync(serviceModel);

            // Assert
            Assert.False(result);

            var existsInDb = await context.JoinRequests.AnyAsync();
            Assert.False(existsInDb);
        }

        [Fact]
        public async Task CreateAsync_WithNonExistentUser_ReturnsFalse()
        {
            // Arrange
            var testUserName = "TestUser";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride
            {
                Title = "TestRide"
            };

            await context.Rides.AddAsync(ride);

            await context.SaveChangesAsync();

            var joinRequestsService = new JoinRequestsService(
                new EfRepository<JoinRequest>(context),
                new EfRepository<Ride>(context),
                new EfRepository<PoolItUser>(context),
                null);

            var serviceModel = new JoinRequestServiceModel
            {
                RideId = ride.Id,
                User = new PoolItUserServiceModel
                {
                    UserName = testUserName
                },
                Message = "Test Message"
            };

            // Act
            var result = await joinRequestsService.CreateAsync(serviceModel);

            // Assert
            Assert.False(result);

            var existsInDb = await context.JoinRequests.AnyAsync();
            Assert.False(existsInDb);
        }

        #endregion

        #region GetReceivedForUserAsync_Tests

        [Fact]
        public async Task GetReceivedForUserAsync_WithRequests_WorksCorrectly()
        {
            // Arrange
            const int expectedCount = 2;

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            await context.Users.AddAsync(user);

            var ride = new Ride
            {
                Title = "Test Ride",
                Car = new Car
                {
                    Owner = user,
                    Model = new CarModel
                    {
                        Manufacturer = new CarManufacturer()
                    }
                },
                Conversation = new Conversation()
            };

            await context.Rides.AddAsync(ride);

            await context.JoinRequests.AddRangeAsync(
                new JoinRequest
                {
                    Ride = ride,
                    User = new PoolItUser
                    {
                        UserName = "OtherUser1"
                    }
                },
                new JoinRequest
                {
                    Ride = ride,
                    User = new PoolItUser
                    {
                        UserName = "OtherUser2"
                    }
                },
                new JoinRequest
                {
                    User = user,
                    Ride = new Ride
                    {
                        Title = "Other Ride",
                        Car = new Car
                        {
                            Owner = new PoolItUser
                            {
                                UserName = "OtherUser3"
                            },
                            Model = new CarModel
                            {
                                Manufacturer = new CarManufacturer()
                            }
                        },
                        Conversation = new Conversation()
                    }
                }
            );

            await context.SaveChangesAsync();

            var joinRequestsService = new JoinRequestsService(new EfRepository<JoinRequest>(context), null, null, null);

            // Act
            var actualCount = (await joinRequestsService.GetReceivedForUserAsync(user.UserName)).Count();

            // Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetReceivedForUserAsync_WithNoRequests_ReturnsEmptyCollection()
        {
            // Arrange
            const int expectedCount = 0;
            const string testUser = "TestUser";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            await context.JoinRequests.AddAsync(new JoinRequest
            {
                User = new PoolItUser
                {
                    UserName = "OtherUser1"
                },
                Ride = new Ride
                {
                    Title = "Other Ride",
                    Car = new Car
                    {
                        Owner = new PoolItUser
                        {
                            UserName = "OtherUser2"
                        },
                        Model = new CarModel
                        {
                            Manufacturer = new CarManufacturer()
                        }
                    },
                    Conversation = new Conversation()
                }
            });

            await context.SaveChangesAsync();

            var joinRequestsService = new JoinRequestsService(new EfRepository<JoinRequest>(context), null, null, null);

            // Act
            var actualCount = (await joinRequestsService.GetReceivedForUserAsync(testUser)).Count();

            // Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetReceivedForUserAsync_WithNullUser_ReturnsNull()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            await context.JoinRequests.AddAsync(new JoinRequest
            {
                User = new PoolItUser
                {
                    UserName = "OtherUser1"
                },
                Ride = new Ride
                {
                    Title = "Other Ride",
                    Car = new Car
                    {
                        Owner = new PoolItUser
                        {
                            UserName = "OtherUser2"
                        },
                        Model = new CarModel
                        {
                            Manufacturer = new CarManufacturer()
                        }
                    },
                    Conversation = new Conversation()
                }
            });

            await context.SaveChangesAsync();

            var joinRequestsService = new JoinRequestsService(new EfRepository<JoinRequest>(context), null, null, null);

            // Act
            var result = await joinRequestsService.GetReceivedForUserAsync(null);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetAsync_Tests

        [Fact]
        public async Task GetAsync_WithCorrectId_WorksCorrectly()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride
            {
                Title = "Test Ride",
                Car = new Car
                {
                    Owner = new PoolItUser
                    {
                        UserName = "TestOwner"
                    },
                    Model = new CarModel
                    {
                        Manufacturer = new CarManufacturer()
                    }
                },
                Conversation = new Conversation()
            };

            await context.Rides.AddAsync(ride);

            var request = new JoinRequest
            {
                Ride = ride,
                User = new PoolItUser
                {
                    UserName = "User1"
                },
                Message = "Test Message 1"
            };

            await context.JoinRequests.AddRangeAsync(
                request,
                new JoinRequest
                {
                    Ride = ride,
                    User = new PoolItUser
                    {
                        UserName = "User2"
                    },
                    Message = "Test Message 2"
                }
            );

            await context.SaveChangesAsync();

            var joinRequestsService = new JoinRequestsService(new EfRepository<JoinRequest>(context), null, null, null);

            // Act
            var result = await joinRequestsService.GetAsync(request.Id);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(request.Message, result.Message);
        }

        [Fact]
        public async Task GetAsync_WithNullId_ReturnsNull()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            await context.JoinRequests.AddAsync(
                new JoinRequest
                {
                    Ride = new Ride
                    {
                        Title = "Test Ride",
                        Car = new Car
                        {
                            Owner = new PoolItUser
                            {
                                UserName = "TestOwner"
                            },
                            Model = new CarModel
                            {
                                Manufacturer = new CarManufacturer()
                            }
                        },
                        Conversation = new Conversation()
                    },
                    User = new PoolItUser
                    {
                        UserName = "TestUser"
                    },
                    Message = "Test Message"
                }
            );

            await context.SaveChangesAsync();

            var joinRequestsService = new JoinRequestsService(new EfRepository<JoinRequest>(context), null, null, null);

            // Act
            var result = await joinRequestsService.GetAsync(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_WithNonExistentRequest_ReturnsNull()
        {
            // Arrange
            var testId = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            await context.JoinRequests.AddAsync(
                new JoinRequest
                {
                    Ride = new Ride
                    {
                        Title = "Test Ride",
                        Car = new Car
                        {
                            Owner = new PoolItUser
                            {
                                UserName = "TestOwner"
                            },
                            Model = new CarModel
                            {
                                Manufacturer = new CarManufacturer()
                            }
                        },
                        Conversation = new Conversation()
                    },
                    User = new PoolItUser
                    {
                        UserName = "TestUser"
                    },
                    Message = "Test Message"
                }
            );

            await context.SaveChangesAsync();

            var joinRequestsService = new JoinRequestsService(new EfRepository<JoinRequest>(context), null, null, null);

            // Act
            var result = await joinRequestsService.GetAsync(testId);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region AcceptAsync_Tests

        [Fact]
        public async Task AcceptAsync_WithCorrectId_WorksCorrectly()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride
            {
                Title = "Test Ride",
                Car = new Car
                {
                    Owner = new PoolItUser
                    {
                        UserName = "TestOwner"
                    },
                    Model = new CarModel
                    {
                        Manufacturer = new CarManufacturer()
                    }
                },
                Conversation = new Conversation()
            };

            var user = new PoolItUser
            {
                UserName = "User1"
            };

            var request = new JoinRequest
            {
                Ride = ride,
                User = user,
                Message = "Test Message"
            };

            await context.JoinRequests.AddAsync(request);

            await context.SaveChangesAsync();

            var joinRequestsService = new JoinRequestsService(new EfRepository<JoinRequest>(context), null, null, new EfRepository<UserRide>(context));

            // Act
            var result = await joinRequestsService.AcceptAsync(request.Id);

            // Assert
            Assert.True(result);

            var joinRequestExists = await context.JoinRequests.AnyAsync();
            Assert.False(joinRequestExists);

            var userRideExists = await context.UserRides.AnyAsync(u => u.UserId == user.Id && u.RideId == ride.Id);
            Assert.True(userRideExists);
        }

        [Fact]
        public async Task AcceptAsync_WithNonExistentRequestRequest_ReturnsFalse()
        {
            // Arrange
            var testRequestId = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var joinRequestsService = new JoinRequestsService(new EfRepository<JoinRequest>(context), null, null, new EfRepository<UserRide>(context));

            // Act
            var result = await joinRequestsService.AcceptAsync(testRequestId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AcceptAsync_WithNullRequestId_ReturnsFalse()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var joinRequestsService = new JoinRequestsService(new EfRepository<JoinRequest>(context), null, null, new EfRepository<UserRide>(context));

            // Act
            var result = await joinRequestsService.AcceptAsync(null);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region DeleteAsync_Tests

        [Fact]
        public async Task DeleteAsync_WithCorrectId_WorksCorrectly()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride
            {
                Title = "Test Ride",
                Car = new Car
                {
                    Owner = new PoolItUser
                    {
                        UserName = "TestOwner"
                    },
                    Model = new CarModel
                    {
                        Manufacturer = new CarManufacturer()
                    }
                },
                Conversation = new Conversation()
            };

            var user = new PoolItUser
            {
                UserName = "User1"
            };

            var request = new JoinRequest
            {
                Ride = ride,
                User = user,
                Message = "Test Message"
            };

            await context.JoinRequests.AddAsync(request);

            await context.SaveChangesAsync();

            var joinRequestsService = new JoinRequestsService(new EfRepository<JoinRequest>(context), null, null, null);

            // Act
            var result = await joinRequestsService.DeleteAsync(request.Id);

            // Assert
            Assert.True(result);

            var joinRequestExists = await context.JoinRequests.AnyAsync();
            Assert.False(joinRequestExists);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistentRequestRequest_ReturnsFalse()
        {
            // Arrange
            var testRequestId = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var joinRequestsService = new JoinRequestsService(new EfRepository<JoinRequest>(context), null, null, null);

            // Act
            var result = await joinRequestsService.DeleteAsync(testRequestId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_WithNullRequestId_ReturnsFalse()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var joinRequestsService = new JoinRequestsService(new EfRepository<JoinRequest>(context), null, null, null);

            // Act
            var result = await joinRequestsService.DeleteAsync(null);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region CanUserSendJoinRequest_Tests

        [Fact]
        public void CanUserSendJoinRequest_WithValidRideAndUserName_ReturnsTrue()
        {
            // Arrange
            const string testUser = "TestUser";

            var ride = new RideServiceModel
            {
                Date = DateTime.UtcNow.AddDays(1),
                Participants = new[]
                {
                    new UserRideServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = "TestUser1"
                        }
                    },
                    new UserRideServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = "TestUser2"
                        }
                    }
                },
                AvailableSeats = 2,
                JoinRequests = new[]
                {
                    new JoinRequestServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = "TestUser3"
                        }
                    }
                }
            };

            var joinRequestsService = new JoinRequestsService(null, null, null, null);

            // Act
            var result = joinRequestsService.CanUserSendJoinRequest(ride, testUser);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanUserSendJoinRequest_WithNullUserName_ReturnsFalse()
        {
            // Arrange
            var ride = new RideServiceModel
            {
                Date = DateTime.UtcNow.AddDays(1),
                Participants = new[]
                {
                    new UserRideServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = "TestUser1"
                        }
                    },
                    new UserRideServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = "TestUser2"
                        }
                    }
                },
                AvailableSeats = 2,
                JoinRequests = new[]
                {
                    new JoinRequestServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = "TestUser3"
                        }
                    }
                }
            };

            var joinRequestsService = new JoinRequestsService(null, null, null, null);

            // Act
            var result = joinRequestsService.CanUserSendJoinRequest(ride, null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanUserSendJoinRequest_WithPastRide_ReturnsFalse()
        {
            // Arrange
            const string testUser = "TestUser";

            var ride = new RideServiceModel
            {
                Date = DateTime.UtcNow.AddDays(-1),
                Participants = new[]
                {
                    new UserRideServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = "TestUser1"
                        }
                    },
                    new UserRideServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = "TestUser2"
                        }
                    }
                },
                AvailableSeats = 2,
                JoinRequests = new[]
                {
                    new JoinRequestServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = "TestUser3"
                        }
                    }
                }
            };

            var joinRequestsService = new JoinRequestsService(null, null, null, null);

            // Act
            var result = joinRequestsService.CanUserSendJoinRequest(ride, testUser);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanUserSendJoinRequest_WithRequestAlreadySent_ReturnsFalse()
        {
            // Arrange
            const string testUser = "TestUser";

            var ride = new RideServiceModel
            {
                Date = DateTime.UtcNow.AddDays(1),
                Participants = new[]
                {
                    new UserRideServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = "TestUser1"
                        }
                    },
                    new UserRideServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = "TestUser2"
                        }
                    }
                },
                AvailableSeats = 2,
                JoinRequests = new[]
                {
                    new JoinRequestServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = "TestUser3"
                        }
                    },
                    new JoinRequestServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = testUser
                        }
                    }
                }
            };

            var joinRequestsService = new JoinRequestsService(null, null, null, null);

            // Act
            var result = joinRequestsService.CanUserSendJoinRequest(ride, testUser);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanUserSendJoinRequest_WithUserAlreadyParticipating_ReturnsFalse()
        {
            // Arrange
            const string testUser = "TestUser";

            var ride = new RideServiceModel
            {
                Date = DateTime.UtcNow.AddDays(1),
                Participants = new[]
                {
                    new UserRideServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = "TestUser1"
                        }
                    },
                    new UserRideServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = "TestUser2"
                        }
                    },
                    new UserRideServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = testUser
                        }
                    }
                },
                AvailableSeats = 2,
                JoinRequests = new[]
                {
                    new JoinRequestServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = "TestUser3"
                        }
                    }
                }
            };

            var joinRequestsService = new JoinRequestsService(null, null, null, null);

            // Act
            var result = joinRequestsService.CanUserSendJoinRequest(ride, testUser);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanUserSendJoinRequest_WithNoFreeSeats_ReturnsFalse()
        {
            // Arrange
            const string testUser = "TestUser";

            var ride = new RideServiceModel
            {
                Date = DateTime.UtcNow.AddDays(1),
                Participants = new[]
                {
                    new UserRideServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = "TestUser1"
                        }
                    },
                    new UserRideServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = "TestUser2"
                        }
                    }
                },
                AvailableSeats = 1,
                JoinRequests = new[]
                {
                    new JoinRequestServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = "TestUser3"
                        }
                    }
                }
            };

            var joinRequestsService = new JoinRequestsService(null, null, null, null);

            // Act
            var result = joinRequestsService.CanUserSendJoinRequest(ride, testUser);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region CanUserAccessRequestAsync_Tests

        [Fact]
        public async Task CanUserAccessRequestAsync_WithUserOrganiser_ReturnsTrue()
        {
            // Arrange
            const string testUser = "TestUser";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var request = new JoinRequest
            {
                User = new PoolItUser
                {
                    UserName = "OtherUser"
                },
                Ride = new Ride
                {
                    Title = "Test Ride",
                    Car = new Car
                    {
                        Owner = new PoolItUser
                        {
                            UserName = testUser
                        },
                        Model = new CarModel
                        {
                            Manufacturer = new CarManufacturer()
                        }
                    },
                    Conversation = new Conversation()
                }
            };

            await context.JoinRequests.AddAsync(request);

            await context.SaveChangesAsync();

            var joinRequestsService = new JoinRequestsService(new EfRepository<JoinRequest>(context), null, null, null);

            // Act
            var result = await joinRequestsService.CanUserAccessRequestAsync(request.Id, testUser);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CanUserAccessRequestAsync_WithOtherUserOrganiser_ReturnsFalse()
        {
            // Arrange
            const string testUser = "TestUser";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var request = new JoinRequest
            {
                User = new PoolItUser
                {
                    UserName = testUser
                },
                Ride = new Ride
                {
                    Title = "Test Ride",
                    Car = new Car
                    {
                        Owner = new PoolItUser
                        {
                            UserName = "OtherUser"
                        },
                        Model = new CarModel
                        {
                            Manufacturer = new CarManufacturer()
                        }
                    },
                    Conversation = new Conversation()
                }
            };

            await context.JoinRequests.AddAsync(request);

            await context.SaveChangesAsync();

            var joinRequestsService = new JoinRequestsService(new EfRepository<JoinRequest>(context), null, null, null);

            // Act
            var result = await joinRequestsService.CanUserAccessRequestAsync(request.Id, testUser);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CanUserAccessRequestAsync_WithNullId_ReturnsFalse()
        {
            // Arrange
            const string testUser = "TestUser";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var joinRequestsService = new JoinRequestsService(new EfRepository<JoinRequest>(context), null, null, null);

            // Act
            var result = await joinRequestsService.CanUserAccessRequestAsync(null, testUser);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CanUserAccessRequestAsync_WithNullUserName_ReturnsFalse()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var request = new JoinRequest
            {
                User = new PoolItUser
                {
                    UserName = "OtherUser"
                },
                Ride = new Ride
                {
                    Title = "Test Ride",
                    Car = new Car
                    {
                        Owner = new PoolItUser
                        {
                            UserName = "TestUser"
                        },
                        Model = new CarModel
                        {
                            Manufacturer = new CarManufacturer()
                        }
                    },
                    Conversation = new Conversation()
                }
            };

            await context.JoinRequests.AddAsync(request);

            await context.SaveChangesAsync();

            var joinRequestsService = new JoinRequestsService(new EfRepository<JoinRequest>(context), null, null, null);

            // Act
            var result = await joinRequestsService.CanUserAccessRequestAsync(request.Id, null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CanUserAccessRequestAsync_WithNonExistentRequest_ReturnsFalse()
        {
            // Arrange
            const string testUser = "TestUser";
            var testId = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var joinRequestsService = new JoinRequestsService(new EfRepository<JoinRequest>(context), null, null, null);

            // Act
            var result = await joinRequestsService.CanUserAccessRequestAsync(testId, testUser);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region GetAllAsync_Tests

        [Fact]
        public async Task GetAllAsync_WithRequests_WorksCorrectly()
        {
            // Arrange
            const int expectedCount = 2;

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride
            {
                Title = "Test Ride",
                Car = new Car
                {
                    Owner = new PoolItUser
                    {
                        UserName = "TestUser"
                    },
                    Model = new CarModel
                    {
                        Manufacturer = new CarManufacturer()
                    }
                },
                Conversation = new Conversation()
            };

            await context.Rides.AddAsync(ride);

            await context.JoinRequests.AddRangeAsync(
                new JoinRequest
                {
                    Ride = ride,
                    User = new PoolItUser
                    {
                        UserName = "OtherUser1"
                    }
                },
                new JoinRequest
                {
                    Ride = ride,
                    User = new PoolItUser
                    {
                        UserName = "OtherUser2"
                    }
                }
            );

            await context.SaveChangesAsync();

            var joinRequestsService = new JoinRequestsService(new EfRepository<JoinRequest>(context), null, null, null);

            // Act
            var actualCount = (await joinRequestsService.GetAllAsync()).Count();

            // Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetAllAsync_WithNoRequests_ReturnsEmptyCollection()
        {
            // Arrange
            const int expectedCount = 0;

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var joinRequestsService = new JoinRequestsService(new EfRepository<JoinRequest>(context), null, null, null);

            // Act
            var actualCount = (await joinRequestsService.GetAllAsync()).Count();

            // Assert
            Assert.Equal(expectedCount, actualCount);
        }

        #endregion
    }
}
