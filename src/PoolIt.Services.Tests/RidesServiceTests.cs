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

    public class RidesServiceTests : BaseTests
    {
        #region CreateAsync_Tests

        [Fact]
        public async Task CreateAsync_WithValidModel_WorksCorrectly()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            var car = new Car
            {
                Model = new CarModel
                {
                    Model = "Test Model",
                    Manufacturer = new CarManufacturer()
                },
                Owner = user,
                Colour = "TestColour"
            };

            await context.Cars.AddAsync(car);

            await context.SaveChangesAsync();

            var serviceModel = new RideServiceModel
            {
                Title = "Test Ride",
                CarId = car.Id,
                From = "Test From",
                To = "Test To",
                Date = DateTime.UtcNow,
                AvailableSeats = 2
            };

            var ridesService = new RidesService(new EfRepository<Ride>(context), null, new EfRepository<Car>(context), null);

            // Act
            var result = await ridesService.CreateAsync(serviceModel);

            // Assert
            Assert.NotNull(result);

            var dbModel = await context.Rides.SingleOrDefaultAsync();
            Assert.NotNull(dbModel);

            // - Check correct id is returned
            Assert.Equal(dbModel.Id, result);

            // - Check Conversation is created
            Assert.NotNull(dbModel.Conversation);

            // - Check organiser is added as participant
            var userRideExists = await context.UserRides.AnyAsync(u => u.UserId == user.Id && u.RideId == dbModel.Id);
            Assert.True(userRideExists);
        }

        [Fact]
        public async Task CreateAsync_WithInvalidModel_ReturnsNull()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var car = new Car
            {
                Model = new CarModel
                {
                    Model = "Test Model",
                    Manufacturer = new CarManufacturer()
                },
                Owner = new PoolItUser
                {
                    UserName = "TestUser"
                },
                Colour = "TestColour"
            };

            await context.Cars.AddAsync(car);

            await context.SaveChangesAsync();

            var serviceModel = new RideServiceModel
            {
                Title = "Test Ride",
                CarId = car.Id
            };

            var ridesService = new RidesService(new EfRepository<Ride>(context), null, new EfRepository<Car>(context), null);

            // Act
            var result = await ridesService.CreateAsync(serviceModel);

            // Assert
            Assert.Null(result);

            var dbModel = await context.Rides.AnyAsync();
            Assert.False(dbModel);
        }

        [Fact]
        public async Task CreateAsync_WithNonExistentCar_ReturnsNull()
        {
            // Arrange
            var testCarId = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var serviceModel = new RideServiceModel
            {
                Title = "Test Ride",
                CarId = testCarId,
                From = "Test From",
                To = "Test To",
                Date = DateTime.UtcNow,
                AvailableSeats = 2
            };

            var ridesService = new RidesService(new EfRepository<Ride>(context), null, new EfRepository<Car>(context), null);

            // Act
            var result = await ridesService.CreateAsync(serviceModel);

            // Assert
            Assert.Null(result);

            var dbModel = await context.Rides.AnyAsync();
            Assert.False(dbModel);
        }

        #endregion

        #region GetAllUpcomingWithFreeSeatsAsync_Tests

        [Fact]
        public async Task GetAllUpcomingWithFreeSeatsAsync_WithRides_WorksCorrectly()
        {
            // Arrange
            const int expectedCount = 1;

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var car = new Car
            {
                Model = new CarModel
                {
                    Model = "Test Model",
                    Manufacturer = new CarManufacturer()
                },
                Owner = new PoolItUser
                {
                    UserName = "TestUser"
                }
            };

            await context.Cars.AddAsync(car);

            await context.Rides.AddRangeAsync(
                new Ride
                {
                    Date = DateTime.UtcNow.AddDays(1),
                    AvailableSeats = 1,
                    Participants = new[]
                    {
                        new UserRide
                        {
                            User = new PoolItUser
                            {
                                UserName = "OtherUser1"
                            }
                        }
                    },
                    Car = car,
                    Conversation = new Conversation()
                },
                new Ride
                {
                    Date = DateTime.UtcNow.AddDays(-1),
                    AvailableSeats = 1,
                    Participants = new[]
                    {
                        new UserRide
                        {
                            User = new PoolItUser
                            {
                                UserName = "OtherUser2"
                            }
                        }
                    },
                    Car = car,
                    Conversation = new Conversation()
                },
                new Ride
                {
                    Date = DateTime.UtcNow.AddDays(1),
                    AvailableSeats = 1,
                    Participants = new[]
                    {
                        new UserRide
                        {
                            User = new PoolItUser
                            {
                                UserName = "OtherUser3"
                            }
                        },
                        new UserRide
                        {
                            User = new PoolItUser
                            {
                                UserName = "OtherUser4"
                            }
                        }
                    },
                    Car = car,
                    Conversation = new Conversation()
                }
            );

            await context.SaveChangesAsync();

            var ridesService = new RidesService(new EfRepository<Ride>(context), null, null, null);

            // Act
            var actualCount = (await ridesService.GetAllUpcomingWithFreeSeatsAsync()).Count();

            // Assert
            Assert.Equal(expectedCount, actualCount);
        }

        #endregion

        #region GetAllUpcomingForUserAsync_Tests

        [Fact]
        public async Task GetAllUpcomingForUserAsync_WithRides_WorksCorrectly()
        {
            // Arrange
            var expectedResult = new[] { "Ride1", "Ride2" };
            const string testUser = "TestUser";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var user = new PoolItUser
            {
                UserName = testUser
            };

            await context.Users.AddAsync(user);

            var car = new Car
            {
                Model = new CarModel
                {
                    Model = "Test Model",
                    Manufacturer = new CarManufacturer()
                },
                Owner = new PoolItUser
                {
                    UserName = "OtherUser1"
                }
            };

            await context.Cars.AddAsync(car);

            await context.Rides.AddRangeAsync(
                new Ride
                {
                    Title = "Ride1",
                    Date = DateTime.UtcNow.AddDays(1),
                    Participants = new[]
                    {
                        new UserRide
                        {
                            User = user
                        }
                    },
                    Car = car,
                    Conversation = new Conversation()
                },
                new Ride
                {
                    Title = "Ride2",
                    Date = DateTime.UtcNow.AddDays(2),
                    Participants = new[]
                    {
                        new UserRide
                        {
                            User = user
                        }
                    },
                    Car = car,
                    Conversation = new Conversation()
                },
                new Ride
                {
                    Title = "OtherRide1",
                    Date = DateTime.UtcNow.AddDays(-1),
                    Participants = new[]
                    {
                        new UserRide
                        {
                            User = user
                        }
                    },
                    Car = car,
                    Conversation = new Conversation()
                },
                new Ride
                {
                    Title = "OtherRide2",
                    Date = DateTime.UtcNow.AddDays(1),
                    Participants = new[]
                    {
                        new UserRide
                        {
                            User = new PoolItUser
                            {
                                UserName = "OtherUser2"
                            }
                        }
                    },
                    Car = car,
                    Conversation = new Conversation()
                }
            );

            await context.SaveChangesAsync();

            var ridesService = new RidesService(new EfRepository<Ride>(context), new EfRepository<PoolItUser>(context), null, null);

            // Act
            var actualResult = (await ridesService.GetAllUpcomingForUserAsync(testUser)).Select(r => r.Title);

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async Task GetAllUpcomingForUserAsync_WithNonExistentUser_ReturnsNull()
        {
            // Arrange
            const string testUser = "TestUser";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ridesService = new RidesService(new EfRepository<Ride>(context), new EfRepository<PoolItUser>(context), null, null);

            // Act
            var result = await ridesService.GetAllUpcomingForUserAsync(testUser);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetAllPastForUserAsync_Tests

        [Fact]
        public async Task GetAllPastForUserAsync_WithRides_WorksCorrectly()
        {
            // Arrange
            var expectedResult = new[] { "Ride1", "Ride2" };
            const string testUser = "TestUser";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var user = new PoolItUser
            {
                UserName = testUser
            };

            await context.Users.AddAsync(user);

            var car = new Car
            {
                Model = new CarModel
                {
                    Model = "Test Model",
                    Manufacturer = new CarManufacturer()
                },
                Owner = new PoolItUser
                {
                    UserName = "OtherUser1"
                }
            };

            await context.Cars.AddAsync(car);

            await context.Rides.AddRangeAsync(
                new Ride
                {
                    Title = "Ride1",
                    Date = DateTime.UtcNow.AddDays(-1),
                    Participants = new[]
                    {
                        new UserRide
                        {
                            User = user
                        }
                    },
                    Car = car,
                    Conversation = new Conversation()
                },
                new Ride
                {
                    Title = "Ride2",
                    Date = DateTime.UtcNow.AddDays(-2),
                    Participants = new[]
                    {
                        new UserRide
                        {
                            User = user
                        }
                    },
                    Car = car,
                    Conversation = new Conversation()
                },
                new Ride
                {
                    Title = "OtherRide1",
                    Date = DateTime.UtcNow.AddDays(1),
                    Participants = new[]
                    {
                        new UserRide
                        {
                            User = user
                        }
                    },
                    Car = car,
                    Conversation = new Conversation()
                },
                new Ride
                {
                    Title = "OtherRide2",
                    Date = DateTime.UtcNow.AddDays(-1),
                    Participants = new[]
                    {
                        new UserRide
                        {
                            User = new PoolItUser
                            {
                                UserName = "OtherUser2"
                            }
                        }
                    },
                    Car = car,
                    Conversation = new Conversation()
                }
            );

            await context.SaveChangesAsync();

            var ridesService = new RidesService(new EfRepository<Ride>(context), new EfRepository<PoolItUser>(context), null, null);

            // Act
            var actualResult = (await ridesService.GetAllPastForUserAsync(testUser)).Select(r => r.Title);

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async Task GetAllPastForUserAsync_WithNonExistentUser_ReturnsNull()
        {
            // Arrange
            const string testUser = "TestUser";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ridesService = new RidesService(new EfRepository<Ride>(context), new EfRepository<PoolItUser>(context), null, null);

            // Act
            var result = await ridesService.GetAllPastForUserAsync(testUser);

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
                Title = "TestRide",
                Car = new Car
                {
                    Model = new CarModel
                    {
                        Model = "Test Model",
                        Manufacturer = new CarManufacturer()
                    },
                    Owner = new PoolItUser
                    {
                        UserName = "TestUser"
                    }
                },
                Conversation = new Conversation()
            };

            await context.Rides.AddAsync(ride);

            await context.SaveChangesAsync();

            var ridesService = new RidesService(new EfRepository<Ride>(context), null, null, null);

            // Act
            var result = await ridesService.GetAsync(ride.Id);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(ride.Title, result.Title);
        }

        [Fact]
        public async Task GetAsync_WithNullId_ReturnsNull()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ridesService = new RidesService(new EfRepository<Ride>(context), null, null, null);

            // Act
            var result = await ridesService.GetAsync(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_WithNonExistentId_ReturnsNull()
        {
            // Arrange
            var testId = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ridesService = new RidesService(new EfRepository<Ride>(context), null, null, null);

            // Act
            var result = await ridesService.GetAsync(testId);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region CanUserAccessRide_Tests

        [Fact]
        public void CanUserAccessRide_WithUpcomingRide_ReturnsTrue()
        {
            // Arrange
            var ride = new RideServiceModel
            {
                Date = DateTime.UtcNow.AddDays(1)
            };

            var ridesService = new RidesService(null, null, null, null);

            // Act
            var result = ridesService.CanUserAccessRide(ride, null);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanUserAccessRide_WithUserParticipant_ReturnsTrue()
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
                            UserName = testUser
                        }
                    }
                }
            };

            var ridesService = new RidesService(null, null, null, null);

            // Act
            var result = ridesService.CanUserAccessRide(ride, testUser);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanUserAccessRide_WithNullUser_ReturnsFalse()
        {
            // Arrange
            var ride = new RideServiceModel
            {
                Date = DateTime.UtcNow.AddDays(-1)
            };

            var ridesService = new RidesService(null, null, null, null);

            // Act
            var result = ridesService.CanUserAccessRide(ride, null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanUserAccessRide_WithUserNotParticipant_ReturnsFalse()
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
                            UserName = "OtherUser"
                        }
                    }
                }
            };

            var ridesService = new RidesService(null, null, null, null);

            // Act
            var result = ridesService.CanUserAccessRide(ride, testUser);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region IsUserOrganiser_Tests

        [Fact]
        public void IsUserOrganiser_WithUserOrganiser_ReturnsTrue()
        {
            // Arrange
            const string testUser = "TestUser";

            var ride = new RideServiceModel
            {
                Car = new CarServiceModel
                {
                    Owner = new PoolItUserServiceModel
                    {
                        UserName = testUser
                    }
                }
            };

            var ridesService = new RidesService(null, null, null, null);

            // Act
            var result = ridesService.IsUserOrganiser(ride, testUser);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsUserOrganiser_WithNullUser_ReturnsFalse()
        {
            // Arrange
            var ride = new RideServiceModel
            {
                Car = new CarServiceModel
                {
                    Owner = new PoolItUserServiceModel
                    {
                        UserName = "OtherUser"
                    }
                }
            };

            var ridesService = new RidesService(null, null, null, null);

            // Act
            var result = ridesService.IsUserOrganiser(ride, null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsUserOrganiser_WithUserNotOrganiser_ReturnsFalse()
        {
            // Arrange
            const string testUser = "TestUser";

            var ride = new RideServiceModel
            {
                Car = new CarServiceModel
                {
                    Owner = new PoolItUserServiceModel
                    {
                        UserName = "OtherUser"
                    }
                }
            };

            var ridesService = new RidesService(null, null, null, null);

            // Act
            var result = ridesService.IsUserOrganiser(ride, testUser);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region IsUserParticipant_Tests

        [Fact]
        public void IsUserParticipant_WithUserParticipant_ReturnsTrue()
        {
            // Arrange
            const string testUser = "TestUser";

            var ride = new RideServiceModel
            {
                Participants = new[]
                {
                    new UserRideServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = testUser
                        }
                    }
                }
            };

            var ridesService = new RidesService(null, null, null, null);

            // Act
            var result = ridesService.IsUserParticipant(ride, testUser);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsUserParticipant_WithNullUser_ReturnsFalse()
        {
            // Arrange
            var ride = new RideServiceModel
            {
                Participants = new[]
                {
                    new UserRideServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = "OtherUser"
                        }
                    }
                }
            };

            var ridesService = new RidesService(null, null, null, null);

            // Act
            var result = ridesService.IsUserParticipant(ride, null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsUserParticipant_WithUserNotParticipant_ReturnsFalse()
        {
            // Arrange
            const string testUser = "TestUser";

            var ride = new RideServiceModel
            {
                Participants = new[]
                {
                    new UserRideServiceModel
                    {
                        User = new PoolItUserServiceModel
                        {
                            UserName = "OtherUser"
                        }
                    }
                }
            };

            var ridesService = new RidesService(null, null, null, null);

            // Act
            var result = ridesService.IsUserParticipant(ride, testUser);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region UpdateAsync_Tests

        [Fact]
        public async Task UpdateAsync_WithValidModel_WorksCorrectly()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride
            {
                Title = "Initial Title",
                PhoneNumber = "0000000000",
                Notes = "Initial Notes",
                Car = new Car(),
                Date = DateTime.UtcNow,
                AvailableSeats = 1,
                From = "Test From",
                To = "Test To"
            };

            await context.Rides.AddAsync(ride);

            await context.SaveChangesAsync();

            var ridesService = new RidesService(new EfRepository<Ride>(context), null, null, null);

            var serviceModel = new RideServiceModel()
            {
                Id = ride.Id,
                Title = "Updated Title",
                PhoneNumber = "0000000001",
                Notes = "Updated Notes",
                CarId = ride.CarId,
                AvailableSeats = ride.AvailableSeats,
                From = ride.From,
                To = ride.To
            };

            // Act
            var result = await ridesService.UpdateAsync(serviceModel);

            // Assert
            Assert.True(result);

            var dbModel = await context.Rides.SingleAsync();

            Assert.Equal(serviceModel.Title, dbModel.Title);
            Assert.Equal(serviceModel.PhoneNumber, dbModel.PhoneNumber);
            Assert.Equal(serviceModel.Notes, dbModel.Notes);
        }

        [Fact]
        public async Task UpdateAsync_WithOtherDataChanged_DoesNotUpdate()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride
            {
                Title = "Initial Title",
                PhoneNumber = "0000000000",
                Notes = "Initial Notes",
                Car = new Car(),
                Date = DateTime.UtcNow,
                AvailableSeats = 2,
                From = "Test From",
                To = "Test To"
            };

            await context.Rides.AddAsync(ride);

            var car = new Car();

            await context.Cars.AddAsync(car);

            await context.SaveChangesAsync();

            var ridesService = new RidesService(new EfRepository<Ride>(context), null, null, null);

            var serviceModel = new RideServiceModel()
            {
                Id = ride.Id,
                Title = ride.Title,
                PhoneNumber = ride.PhoneNumber,
                Notes = ride.Notes,
                CarId = car.Id,
                AvailableSeats = 1,
                From = "Updated From",
                To = "Updated To"
            };

            var initialDate = ride.Date;
            var initialSeats = ride.AvailableSeats;
            var initialFrom = ride.From;
            var initialTo = ride.To;

            // Act
            await ridesService.UpdateAsync(serviceModel);

            // Assert
            var dbModel = await context.Rides.SingleAsync();

            Assert.Equal(initialDate, dbModel.Date);
            Assert.Equal(initialSeats, dbModel.AvailableSeats);
            Assert.Equal(initialFrom, dbModel.From);
            Assert.Equal(initialTo, dbModel.To);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidModel_ReturnsFalse()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride
            {
                Title = "Initial Title",
                PhoneNumber = "0000000000",
                Notes = "Initial Notes",
                Car = new Car(),
                Date = DateTime.UtcNow,
                AvailableSeats = 1,
                From = "Test From",
                To = "Test To"
            };

            await context.Rides.AddAsync(ride);

            await context.SaveChangesAsync();

            var ridesService = new RidesService(new EfRepository<Ride>(context), null, null, null);

            var serviceModel = new RideServiceModel()
            {
                Id = ride.Id,
                Notes = "Updated Notes",
                CarId = ride.CarId,
                AvailableSeats = ride.AvailableSeats,
                From = ride.From,
                To = ride.To
            };

            var initialTitle = ride.Title;

            // Act
            var result = await ridesService.UpdateAsync(serviceModel);

            // Assert
            Assert.False(result);

            var dbModel = await context.Rides.SingleAsync();

            Assert.Equal(initialTitle, dbModel.Title);
        }

        [Fact]
        public async Task UpdateAsync_WithNullId_ReturnsFalse()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride
            {
                Title = "Initial Title",
                PhoneNumber = "0000000000",
                Notes = "Initial Notes",
                Car = new Car(),
                Date = DateTime.UtcNow,
                AvailableSeats = 1,
                From = "Test From",
                To = "Test To"
            };

            await context.Rides.AddAsync(ride);

            await context.SaveChangesAsync();

            var ridesService = new RidesService(new EfRepository<Ride>(context), null, null, null);

            var serviceModel = new RideServiceModel()
            {
                Title = "Updated Title",
                PhoneNumber = "0000000001",
                Notes = "Updated Notes",
                CarId = ride.CarId,
                AvailableSeats = ride.AvailableSeats,
                From = ride.From,
                To = ride.To
            };

            // Act
            var result = await ridesService.UpdateAsync(serviceModel);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentId_ReturnsFalse()
        {
            // Arrange
            var testRideId = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ride = new Ride
            {
                Title = "Initial Title",
                PhoneNumber = "0000000000",
                Notes = "Initial Notes",
                Car = new Car(),
                Date = DateTime.UtcNow,
                AvailableSeats = 1,
                From = "Test From",
                To = "Test To"
            };

            await context.Rides.AddAsync(ride);

            await context.SaveChangesAsync();

            var ridesService = new RidesService(new EfRepository<Ride>(context), null, null, null);

            var serviceModel = new RideServiceModel()
            {
                Id = testRideId,
                Title = "Updated Title",
                PhoneNumber = "0000000001",
                Notes = "Updated Notes",
                CarId = ride.CarId,
                AvailableSeats = ride.AvailableSeats,
                From = ride.From,
                To = ride.To
            };

            // Act
            var result = await ridesService.UpdateAsync(serviceModel);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region GetAllAsync_Tests

        [Fact]
        public async Task GetAllAsync_WithRides_WorksCorrectly()
        {
            // Arrange
            const int expectedCount = 2;

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var car = new Car
            {
                Model = new CarModel
                {
                    Model = "Test Model",
                    Manufacturer = new CarManufacturer()
                },
                Owner = new PoolItUser
                {
                    UserName = "TestUser"
                }
            };

            await context.Cars.AddAsync(car);

            await context.Rides.AddRangeAsync(
                new Ride
                {
                    Date = DateTime.UtcNow.AddDays(1),
                    AvailableSeats = 1,
                    Car = car,
                    Conversation = new Conversation()
                },
                new Ride
                {
                    Date = DateTime.UtcNow.AddDays(-1),
                    AvailableSeats = 1,
                    Car = car,
                    Conversation = new Conversation()
                }
            );

            await context.SaveChangesAsync();

            var ridesService = new RidesService(new EfRepository<Ride>(context), null, null, null);

            // Act
            var actualCount = (await ridesService.GetAllAsync()).Count();

            // Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetAllAsync_WithNoRides_ReturnsEmptyCollection()
        {
            // Arrange
            const int expectedCount = 0;

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ridesService = new RidesService(new EfRepository<Ride>(context), null, null, null);

            // Act
            var actualCount = (await ridesService.GetAllAsync()).Count();

            // Assert
            Assert.Equal(expectedCount, actualCount);
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
                Title = "Ride Title",
                PhoneNumber = "0000000000",
                Notes = "Ride Notes",
                Car = new Car(),
                Date = DateTime.UtcNow,
                AvailableSeats = 1,
                From = "Test From",
                To = "Test To",
                Conversation = new Conversation()
            };

            await context.Rides.AddAsync(ride);

            await context.SaveChangesAsync();

            var ridesService = new RidesService(new EfRepository<Ride>(context), null, null, new EfRepository<Conversation>(context));

            // Act
            var result = await ridesService.DeleteAsync(ride.Id);

            // Assert
            Assert.True(result);

            var rideExists = await context.Rides.AnyAsync();
            Assert.False(rideExists);

            var conversationExists = await context.Conversations.AnyAsync();
            Assert.False(conversationExists);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistentRequestRide_ReturnsFalse()
        {
            // Arrange
            var testRideId = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var ridesService = new RidesService(new EfRepository<Ride>(context), null, null, new EfRepository<Conversation>(context));

            // Act
            var result = await ridesService.DeleteAsync(testRideId);

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

            var ridesService = new RidesService(new EfRepository<Ride>(context), null, null, new EfRepository<Conversation>(context));

            // Act
            var result = await ridesService.DeleteAsync(null);

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}
