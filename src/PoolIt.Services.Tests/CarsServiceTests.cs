namespace PoolIt.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Data.Repository;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using PoolIt.Models;
    using Xunit;

    public class CarsServiceTests : BaseTests
    {
        #region CreateAsync_Tests

        [Fact]
        public async Task CreateAsync_WithValidModel_WorksCorrectly()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var model = new CarModel
            {
                Model = "Test Model"
            };

            await context.CarModels.AddAsync(model);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            await context.Users.AddAsync(user);

            await context.SaveChangesAsync();

            var carsService = new CarsService(new EfRepository<Car>(context), new EfRepository<CarModel>(context), new EfRepository<PoolItUser>(context));

            var serviceModel = new CarServiceModel
            {
                ModelId = model.Id,
                Colour = "Test Colour",
                Owner = new PoolItUserServiceModel
                {
                    UserName = user.UserName
                }
            };

            // Act
            var result = await carsService.CreateAsync(serviceModel);

            // Assert
            Assert.True(result);

            var dbModel = await context.Cars.SingleOrDefaultAsync();
            Assert.NotNull(dbModel);

            Assert.Equal(user.Id, dbModel.OwnerId);
        }

        [Fact]
        public async Task CreateAsync_WithInvalidModel_ReturnsFalse()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var model = new CarModel
            {
                Model = "Test Model"
            };

            await context.CarModels.AddAsync(model);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            await context.Users.AddAsync(user);

            await context.SaveChangesAsync();

            var carsService = new CarsService(new EfRepository<Car>(context), new EfRepository<CarModel>(context), new EfRepository<PoolItUser>(context));

            var serviceModel = new CarServiceModel
            {
                ModelId = model.Id,
                Owner = new PoolItUserServiceModel
                {
                    UserName = user.UserName
                }
            };

            // Act
            var result = await carsService.CreateAsync(serviceModel);

            // Assert
            Assert.False(result);

            var dbModelAdded = await context.Cars.AnyAsync();
            Assert.False(dbModelAdded);
        }

        [Fact]
        public async Task CreateAsync_WithNonExistentModelId_ReturnsFalse()
        {
            // Arrange
            var testModelId = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            await context.Users.AddAsync(user);

            await context.SaveChangesAsync();

            var carsService = new CarsService(new EfRepository<Car>(context), new EfRepository<CarModel>(context), new EfRepository<PoolItUser>(context));

            var serviceModel = new CarServiceModel
            {
                ModelId = testModelId,
                Colour = "Test Colour",
                Owner = new PoolItUserServiceModel
                {
                    UserName = user.UserName
                }
            };

            // Act
            var result = await carsService.CreateAsync(serviceModel);

            // Assert
            Assert.False(result);

            var dbModelAdded = await context.Cars.AnyAsync();
            Assert.False(dbModelAdded);
        }

        [Fact]
        public async Task CreateAsync_WithNonExistentUser_ReturnsFalse()
        {
            // Arrange
            const string testUser = "TestUser";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var model = new CarModel
            {
                Model = "Test Model"
            };

            await context.CarModels.AddAsync(model);

            await context.SaveChangesAsync();

            var carsService = new CarsService(new EfRepository<Car>(context), new EfRepository<CarModel>(context), new EfRepository<PoolItUser>(context));

            var serviceModel = new CarServiceModel
            {
                ModelId = model.Id,
                Colour = "Test Colour",
                Owner = new PoolItUserServiceModel
                {
                    UserName = testUser
                }
            };

            // Act
            var result = await carsService.CreateAsync(serviceModel);

            // Assert
            Assert.False(result);

            var dbModelAdded = await context.Cars.AnyAsync();
            Assert.False(dbModelAdded);
        }

        #endregion

        #region GetAllForUserAsync_Tests

        [Fact]
        public async Task GetAllForUserAsync_WithCorrectUser_WorksCorrectly()
        {
            // Arrange
            const int expectedCount = 2;

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var model = new CarModel
            {
                Model = "Test Model",
                Manufacturer = new CarManufacturer()
            };

            await context.CarModels.AddAsync(model);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            await context.Users.AddAsync(user);

            await context.Cars.AddRangeAsync(
                new Car
                {
                    Colour = "Test Colour 1",
                    ModelId = model.Id,
                    OwnerId = user.Id
                },
                new Car
                {
                    Colour = "Test Colour 2",
                    ModelId = model.Id,
                    OwnerId = user.Id
                },
                new Car
                {
                    Colour = "Test Colour 3",
                    ModelId = model.Id,
                    Owner = new PoolItUser
                    {
                        UserName = "Other User 1"
                    }
                }
            );

            await context.SaveChangesAsync();

            var carsService = new CarsService(new EfRepository<Car>(context), null, new EfRepository<PoolItUser>(context));

            // Act
            var actualCount = (await carsService.GetAllForUserAsync(user.UserName)).Count();

            // Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetAllForUserAsync_WithNullUserId_ReturnsNull()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var model = new CarModel
            {
                Model = "Test Model",
                Manufacturer = new CarManufacturer()
            };

            await context.CarModels.AddAsync(model);

            await context.Cars.AddRangeAsync(
                new Car
                {
                    Colour = "Test Colour 3",
                    ModelId = model.Id,
                    Owner = new PoolItUser
                    {
                        UserName = "Other User 1"
                    }
                },
                new Car
                {
                    Colour = "Test Colour 4",
                    ModelId = model.Id,
                    Owner = new PoolItUser
                    {
                        UserName = "Other User 2"
                    }
                }
            );

            await context.SaveChangesAsync();

            var carsService = new CarsService(new EfRepository<Car>(context), null, new EfRepository<PoolItUser>(context));

            // Act
            var result = await carsService.GetAllForUserAsync(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllForUserAsync_WithNonExistentUser_ReturnsNull()
        {
            // Arrange
            var testUserName = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var model = new CarModel
            {
                Model = "Test Model",
                Manufacturer = new CarManufacturer()
            };

            await context.CarModels.AddAsync(model);

            await context.Cars.AddRangeAsync(
                new Car
                {
                    Colour = "Test Colour 3",
                    ModelId = model.Id,
                    Owner = new PoolItUser
                    {
                        UserName = "Other User 1"
                    }
                },
                new Car
                {
                    Colour = "Test Colour 4",
                    ModelId = model.Id,
                    Owner = new PoolItUser
                    {
                        UserName = "Other User 2"
                    }
                }
            );

            await context.SaveChangesAsync();

            var carsService = new CarsService(new EfRepository<Car>(context), null, new EfRepository<PoolItUser>(context));

            // Act
            var result = await carsService.GetAllForUserAsync(testUserName);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetAllAsync_Tests

        [Fact]
        public async Task GetAllAsync_WithCars_WorksCorrectly()
        {
            // Arrange
            const int expectedCount = 3;

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var model = new CarModel
            {
                Model = "Test Model",
                Manufacturer = new CarManufacturer()
            };

            await context.CarModels.AddAsync(model);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            await context.Users.AddAsync(user);

            await context.Cars.AddRangeAsync(
                new Car
                {
                    Colour = "Test Colour 1",
                    ModelId = model.Id,
                    OwnerId = user.Id
                },
                new Car
                {
                    Colour = "Test Colour 2",
                    ModelId = model.Id,
                    OwnerId = user.Id
                },
                new Car
                {
                    Colour = "Test Colour 3",
                    ModelId = model.Id,
                    OwnerId = user.Id
                }
            );

            await context.SaveChangesAsync();

            var carsService = new CarsService(new EfRepository<Car>(context), null, null);

            // Act
            var actualCount = (await carsService.GetAllAsync()).Count();

            // Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetAllAsync_WithNoCars_ReturnsEmptyEnumerable()
        {
            // Arrange
            const int expectedCount = 0;

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var carsService = new CarsService(new EfRepository<Car>(context), null, null);

            // Act
            var actualCount = (await carsService.GetAllAsync()).Count();

            // Assert
            Assert.Equal(expectedCount, actualCount);
        }

        #endregion

        #region GetAsync_Tests

        [Fact]
        public async Task GetAsync_WithCorrectCarId_WorksCorrectly()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var model = new CarModel
            {
                Model = "Test Model",
                Manufacturer = new CarManufacturer()
            };

            await context.CarModels.AddAsync(model);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            await context.Users.AddAsync(user);

            var car = new Car
            {
                Colour = "Test Colour 1",
                ModelId = model.Id,
                OwnerId = user.Id
            };

            await context.Cars.AddRangeAsync(
                car,
                new Car
                {
                    Colour = "Test Colour 2",
                    ModelId = model.Id,
                    OwnerId = user.Id
                },
                new Car
                {
                    Colour = "Test Colour 3",
                    ModelId = model.Id,
                    OwnerId = user.Id
                }
            );

            await context.SaveChangesAsync();

            var carsService = new CarsService(new EfRepository<Car>(context), null, null);

            // Act
            var result = await carsService.GetAsync(car.Id);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(car.Colour, result.Colour);
        }

        [Fact]
        public async Task GetAsync_WithNonExistentCar_ReturnsNull()
        {
            // Arrange
            var testCarId = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var model = new CarModel
            {
                Model = "Test Model",
                Manufacturer = new CarManufacturer()
            };

            await context.CarModels.AddAsync(model);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            await context.Users.AddAsync(user);

            await context.Cars.AddRangeAsync(
                new Car
                {
                    Colour = "Test Colour 1",
                    ModelId = model.Id,
                    OwnerId = user.Id
                },
                new Car
                {
                    Colour = "Test Colour 2",
                    ModelId = model.Id,
                    OwnerId = user.Id
                },
                new Car
                {
                    Colour = "Test Colour 3",
                    ModelId = model.Id,
                    OwnerId = user.Id
                }
            );

            await context.SaveChangesAsync();

            var carsService = new CarsService(new EfRepository<Car>(context), null, null);

            // Act
            var result = await carsService.GetAsync(testCarId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_WithNullCarId_ReturnsNull()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var model = new CarModel
            {
                Model = "Test Model",
                Manufacturer = new CarManufacturer()
            };

            await context.CarModels.AddAsync(model);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            await context.Users.AddAsync(user);

            await context.Cars.AddRangeAsync(
                new Car
                {
                    Colour = "Test Colour 1",
                    ModelId = model.Id,
                    OwnerId = user.Id
                },
                new Car
                {
                    Colour = "Test Colour 2",
                    ModelId = model.Id,
                    OwnerId = user.Id
                },
                new Car
                {
                    Colour = "Test Colour 3",
                    ModelId = model.Id,
                    OwnerId = user.Id
                }
            );

            await context.SaveChangesAsync();

            var carsService = new CarsService(new EfRepository<Car>(context), null, null);

            // Act
            var result = await carsService.GetAsync(null);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region IsUserOwner_Tests

        [Fact]
        public void IsUserOwner_WithCorrectUserOwner_ReturnsTrue()
        {
            // Arrange
            const string testUserName = "TestUser";

            var serviceModel = new CarServiceModel
            {
                Owner = new PoolItUserServiceModel
                {
                    UserName = testUserName
                }
            };

            var carsService = new CarsService(null, null, null);

            // Act
            var result = carsService.IsUserOwner(serviceModel, testUserName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsUserOwner_WithOtherUserOwner_ReturnsFalse()
        {
            // Arrange
            const string testUserName = "TestUser";

            var serviceModel = new CarServiceModel
            {
                Owner = new PoolItUserServiceModel
                {
                    UserName = "OtherUser"
                }
            };

            var carsService = new CarsService(null, null, null);

            // Act
            var result = carsService.IsUserOwner(serviceModel, testUserName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsUserOwner_WithNullUserName_ReturnsFalse()
        {
            // Arrange
            var serviceModel = new CarServiceModel
            {
                Owner = new PoolItUserServiceModel
                {
                    UserName = "TestUser"
                }
            };

            var carsService = new CarsService(null, null, null);

            // Act
            var result = carsService.IsUserOwner(serviceModel, null);

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

            var model = new CarModel
            {
                Model = "Test Model"
            };

            await context.CarModels.AddAsync(model);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            await context.Users.AddAsync(user);

            var car = new Car
            {
                ModelId = model.Id,
                Colour = "Test Colour",
                OwnerId = user.Id
            };

            await context.Cars.AddAsync(car);

            await context.SaveChangesAsync();

            var carsService = new CarsService(new EfRepository<Car>(context), null, null);

            var serviceModel = new CarServiceModel
            {
                Id = car.Id,
                ModelId = car.ModelId,
                OwnerId = car.OwnerId,
                Colour = "Updated Colour",
                Details = "Updated Details"
            };

            // Act
            var result = await carsService.UpdateAsync(serviceModel);

            // Assert
            Assert.True(result);

            var dbModel = await context.Cars.SingleAsync();

            Assert.Equal(serviceModel.Colour, dbModel.Colour);
            Assert.Equal(serviceModel.Details, dbModel.Details);
        }

        [Fact]
        public async Task UpdateAsync_WithModelAndUserChanged_DoesNotUpdate()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var model = new CarModel
            {
                Model = "Test Model"
            };

            var newModel = new CarModel
            {
                Model = "New Model"
            };

            await context.CarModels.AddRangeAsync(model, newModel);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            var newUser = new PoolItUser
            {
                UserName = "NewUser"
            };

            await context.Users.AddRangeAsync(user, newUser);

            var car = new Car
            {
                ModelId = model.Id,
                Colour = "Test Colour",
                OwnerId = user.Id
            };

            await context.Cars.AddAsync(car);

            await context.SaveChangesAsync();

            var carsService = new CarsService(new EfRepository<Car>(context), null, null);

            var serviceModel = new CarServiceModel
            {
                Id = car.Id,
                ModelId = newModel.Id,
                OwnerId = newUser.Id,
                Colour = "Updated Colour",
                Details = "Updated Details"
            };

            // Act
            await carsService.UpdateAsync(serviceModel);

            // Assert
            var dbModel = await context.Cars.SingleAsync();

            Assert.Equal(user.Id, dbModel.OwnerId);
            Assert.Equal(model.Id, dbModel.ModelId);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidModel_ReturnsFalse()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var model = new CarModel
            {
                Model = "Test Model"
            };

            await context.CarModels.AddAsync(model);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            await context.Users.AddAsync(user);

            var car = new Car
            {
                ModelId = model.Id,
                OwnerId = user.Id,
                Colour = "Test Colour",
                Details = "Test Details"
            };

            await context.Cars.AddAsync(car);

            await context.SaveChangesAsync();

            var carsService = new CarsService(new EfRepository<Car>(context), null, null);

            var serviceModel = new CarServiceModel
            {
                Id = car.Id,
                Details = "Updated Details"
            };

            // Act
            var result = await carsService.UpdateAsync(serviceModel);

            // Assert
            Assert.False(result);

            var dbModel = await context.Cars.SingleAsync();

            Assert.Equal(car.Details, dbModel.Details);
        }

        [Fact]
        public async Task UpdateAsync_WithNullId_ReturnsFalse()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var model = new CarModel
            {
                Model = "Test Model"
            };

            await context.CarModels.AddAsync(model);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            await context.Users.AddAsync(user);

            var car = new Car
            {
                ModelId = model.Id,
                OwnerId = user.Id,
                Colour = "Test Colour",
                Details = "Test Details"
            };

            await context.Cars.AddAsync(car);

            await context.SaveChangesAsync();

            var carsService = new CarsService(new EfRepository<Car>(context), null, null);

            var serviceModel = new CarServiceModel
            {
                Id = null,
                ModelId = car.ModelId,
                OwnerId = car.OwnerId,
                Colour = "Updated Colour",
                Details = "Updated Details"
            };

            // Act
            var result = await carsService.UpdateAsync(serviceModel);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentCarId_ReturnsFalse()
        {
            // Arrange
            var testCarId = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var model = new CarModel
            {
                Model = "Test Model"
            };

            await context.CarModels.AddAsync(model);

            var user = new PoolItUser
            {
                UserName = "TestUser"
            };

            await context.Users.AddAsync(user);

            var car = new Car
            {
                ModelId = model.Id,
                OwnerId = user.Id,
                Colour = "Test Colour",
                Details = "Test Details"
            };

            await context.Cars.AddAsync(car);

            await context.SaveChangesAsync();

            var carsService = new CarsService(new EfRepository<Car>(context), null, null);

            var serviceModel = new CarServiceModel
            {
                Id = testCarId,
                ModelId = car.ModelId,
                OwnerId = car.OwnerId,
                Colour = "Updated Colour",
                Details = "Updated Details"
            };

            // Act
            var result = await carsService.UpdateAsync(serviceModel);

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

            var car = new Car
            {
                Model = new CarModel
                {
                    Model = "Test Model"
                },
                Owner = new PoolItUser
                {
                    UserName = "TestUser"
                },
                Colour = "Test Colour",
            };

            await context.Cars.AddAsync(car);

            context.SaveChanges();

            var carsService = new CarsService(new EfRepository<Car>(context), null, null);

            // Act
            var result = await carsService.DeleteAsync(car.Id);

            // Assert
            Assert.True(result);

            var existsInDb = await context.Cars.AnyAsync();
            Assert.False(existsInDb);
        }

        [Fact]
        public async Task DeleteAsync_WithNullId_ReturnsFalse()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var car = new Car
            {
                Model = new CarModel
                {
                    Model = "Test Model"
                },
                Owner = new PoolItUser
                {
                    UserName = "TestUser"
                },
                Colour = "Test Colour"
            };

            await context.Cars.AddAsync(car);

            context.SaveChanges();

            var carsService = new CarsService(new EfRepository<Car>(context), null, null);

            // Act
            var result = await carsService.DeleteAsync(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_WithRides_DoesNotDelete()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var car = new Car
            {
                Model = new CarModel
                {
                    Model = "Test Model"
                },
                Owner = new PoolItUser
                {
                    UserName = "TestUser"
                },
                Colour = "Test Colour",
            };

            await context.Cars.AddAsync(car);

            await context.Rides.AddAsync(new Ride
            {
                Title = "Test Ride",
                Car = car
            });

            context.SaveChanges();

            var carsService = new CarsService(new EfRepository<Car>(context), null, null);

            // Act
            var result = await carsService.DeleteAsync(car.Id);

            // Assert
            Assert.False(result);

            var existsInDb = await context.Cars.AnyAsync();
            Assert.True(existsInDb);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistentId_ReturnsFalse()
        {
            // Arrange
            var testId = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var car = new Car
            {
                Model = new CarModel
                {
                    Model = "Test Model"
                },
                Owner = new PoolItUser
                {
                    UserName = "TestUser"
                },
                Colour = "Test Colour"
            };

            await context.Cars.AddAsync(car);

            context.SaveChanges();

            var carsService = new CarsService(new EfRepository<Car>(context), null, null);

            // Act
            var result = await carsService.DeleteAsync(testId);

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}
