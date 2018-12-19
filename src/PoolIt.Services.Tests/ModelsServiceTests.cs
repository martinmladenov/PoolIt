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

    public class ModelsServiceTests : BaseTests
    {
        #region CreateAsync_Tests

        [Fact]
        public async Task CreateAsync_WithValidModel_WorksCorrectly()
        {
            // Arrange
            const string testName = "TestName";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var modelsService = new ModelsService(new EfRepository<CarModel>(context));

            var serviceModel = new CarModelServiceModel
            {
                Model = testName
            };

            // Act
            var result = await modelsService.CreateAsync(serviceModel);

            // Assert
            Assert.True(result);

            var addedToDb = await context.CarModels.AnyAsync(m => m.Model == testName);
            Assert.True(addedToDb);
        }

        [Fact]
        public async Task CreateAsync_WithInvalidModel_ReturnsFalse()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var modelsService = new ModelsService(new EfRepository<CarModel>(context));

            var serviceModel = new CarModelServiceModel();

            // Act
            var result = await modelsService.CreateAsync(serviceModel);

            // Assert
            Assert.False(result);

            var dbCount = await context.CarModels.CountAsync();
            Assert.Equal(0, dbCount);
        }

        #endregion

        #region DeleteAsync_Tests

        [Fact]
        public async Task DeleteAsync_WithCorrectId_WorksCorrectly()
        {
            // Arrange
            const string testName = "TestName";

            var model = new CarModel
            {
                Model = testName
            };

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarModels.AddRange(
                model,
                new CarModel
                {
                    Model = "OtherModel1"
                },
                new CarModel
                {
                    Model = "OtherModel2"
                });

            context.SaveChanges();

            var modelsService = new ModelsService(new EfRepository<CarModel>(context));

            // Act
            var result = await modelsService.DeleteAsync(model.Id);

            // Assert
            Assert.True(result);

            var existsInDb = await context.CarModels.AnyAsync(m => m.Model == testName);
            Assert.False(existsInDb);

            var dbCount = await context.CarModels.CountAsync();
            Assert.Equal(2, dbCount);
        }

        [Fact]
        public async Task DeleteAsync_WithNullId_ReturnsFalse()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarModels.AddRange(
                new CarModel
                {
                    Model = "OtherModel1"
                },
                new CarModel
                {
                    Model = "OtherModel2"
                });

            context.SaveChanges();

            var modelsService = new ModelsService(new EfRepository<CarModel>(context));

            // Act
            var result = await modelsService.DeleteAsync(null);

            // Assert
            Assert.False(result);

            var dbCount = await context.CarModels.CountAsync();
            Assert.Equal(2, dbCount);
        }

        [Fact]
        public async Task DeleteAsync_WithCars_DoesNotDelete()
        {
            // Arrange
            const string testName = "TestName";

            var model = new CarModel
            {
                Model = testName,
                Cars = new List<Car>
                {
                    new Car
                    {
                        Colour = "Green"
                    }
                }
            };

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarModels.Add(model);

            context.SaveChanges();

            var modelsService = new ModelsService(new EfRepository<CarModel>(context));

            // Act
            var result = await modelsService.DeleteAsync(model.Id);

            // Assert
            Assert.False(result);

            var existsInDb = await context.CarModels.AnyAsync(m => m.Model == testName);
            Assert.True(existsInDb);

            var dbCount = await context.CarModels.CountAsync();
            Assert.Equal(1, dbCount);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistentId_ReturnsFalse()
        {
            // Arrange
            var testId = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarModels.AddRange(
                new CarModel
                {
                    Model = "OtherModel1"
                },
                new CarModel
                {
                    Model = "OtherModel2"
                });

            context.SaveChanges();

            var modelsService = new ModelsService(new EfRepository<CarModel>(context));

            // Act
            var result = await modelsService.DeleteAsync(testId);

            // Assert
            Assert.False(result);

            var dbCount = await context.CarModels.CountAsync();
            Assert.Equal(2, dbCount);
        }

        #endregion

        #region UpdateAsync_Tests

        [Fact]
        public async Task UpdateAsync_WithValidModel_WorksCorrectly()
        {
            // Arrange
            const string expectedResult = "TestName";

            var model = new CarModel
            {
                Model = "InitialName"
            };

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarModels.AddRange(
                model,
                new CarModel
                {
                    Model = "OtherModel1"
                },
                new CarModel
                {
                    Model = "OtherModel2"
                });

            context.SaveChanges();

            var modelsService = new ModelsService(new EfRepository<CarModel>(context));

            var serviceModel = new CarModelServiceModel
            {
                Id = model.Id,
                Model = expectedResult
            };

            // Act
            var result = await modelsService.UpdateAsync(serviceModel);

            // Assert
            Assert.True(result);

            var actualResult = (await context.CarModels.SingleAsync(m => m.Id == model.Id)).Model;
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidModel_DoesNotChange()
        {
            // Arrange
            const string expectedResult = "InitialName";

            var model = new CarModel
            {
                Model = expectedResult
            };

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarModels.AddRange(
                model,
                new CarModel
                {
                    Model = "OtherModel1"
                },
                new CarModel
                {
                    Model = "OtherModel2"
                });

            context.SaveChanges();

            var modelsService = new ModelsService(new EfRepository<CarModel>(context));

            var serviceModel = new CarModelServiceModel
            {
                Id = model.Id,
                Model = "a"
            };

            // Act
            var result = await modelsService.UpdateAsync(serviceModel);

            // Assert
            Assert.False(result);

            var actualResult = (await context.CarModels.SingleAsync(m => m.Id == model.Id)).Model;
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async Task UpdateAsync_WithNullId_ReturnsFalse()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarModels.AddRange(
                new CarModel
                {
                    Model = "OtherModel1"
                },
                new CarModel
                {
                    Model = "OtherModel2"
                });

            context.SaveChanges();

            var modelsService = new ModelsService(new EfRepository<CarModel>(context));

            var serviceModel = new CarModelServiceModel
            {
                Id = null,
                Model = "TestName"
            };

            // Act
            var result = await modelsService.UpdateAsync(serviceModel);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentId_ReturnsFalse()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarModels.AddRange(
                new CarModel
                {
                    Model = "OtherModel1"
                },
                new CarModel
                {
                    Model = "OtherModel2"
                });

            context.SaveChanges();

            var modelsService = new ModelsService(new EfRepository<CarModel>(context));

            var serviceModel = new CarModelServiceModel
            {
                Id = Guid.NewGuid().ToString(),
                Model = "TestName"
            };

            // Act
            var result = await modelsService.UpdateAsync(serviceModel);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region ExistsAsync_Tests

        [Fact]
        public async Task ExistsAsync_WithExistentModel_ReturnsTrue()
        {
            // Arrange
            const string testName = "TestName";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var manufacturer = new CarManufacturer
            {
                Name = "TestManufacturer"
            };

            context.CarModels.AddRange(
                new CarModel
                {
                    Model = testName,
                    Manufacturer = manufacturer
                },
                new CarModel
                {
                    Model = "OtherModel",
                    Manufacturer = manufacturer
                });

            context.SaveChanges();

            var modelsService = new ModelsService(new EfRepository<CarModel>(context));

            var serviceModel = new CarModelServiceModel
            {
                Model = testName,
                ManufacturerId = manufacturer.Id
            };

            // Act
            var result = await modelsService.ExistsAsync(serviceModel);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_WithUppercaseName_ReturnsTrue()
        {
            // Arrange
            const string testName = "TestName";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var manufacturer = new CarManufacturer
            {
                Name = "TestManufacturer"
            };

            context.CarModels.AddRange(
                new CarModel
                {
                    Model = testName,
                    Manufacturer = manufacturer
                },
                new CarModel
                {
                    Model = "OtherModel",
                    Manufacturer = manufacturer
                });

            context.SaveChanges();

            var modelsService = new ModelsService(new EfRepository<CarModel>(context));

            var serviceModel = new CarModelServiceModel
            {
                Model = testName.ToUpper(),
                ManufacturerId = manufacturer.Id
            };

            // Act
            var result = await modelsService.ExistsAsync(serviceModel);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_WithNonExistentName_ReturnsFalse()
        {
            // Arrange
            const string testName = "TestName";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var manufacturer = new CarManufacturer
            {
                Name = "TestManufacturer"
            };

            context.CarModels.AddRange(
                new CarModel
                {
                    Model = "Model1",
                    Manufacturer = manufacturer
                },
                new CarModel
                {
                    Model = "Model2",
                    Manufacturer = manufacturer
                });

            context.SaveChanges();

            var modelsService = new ModelsService(new EfRepository<CarModel>(context));

            var serviceModel = new CarModelServiceModel
            {
                Model = testName,
                ManufacturerId = manufacturer.Id
            };

            // Act
            var result = await modelsService.ExistsAsync(serviceModel);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExistsAsync_WithDifferentManufacturer_ReturnsFalse()
        {
            // Arrange
            const string testName = "TestName";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var manufacturer = new CarManufacturer
            {
                Name = "TestManufacturer"
            };

            context.CarModels.AddRange(
                new CarModel
                {
                    Model = testName,
                    Manufacturer = new CarManufacturer
                    {
                        Name = "OtherManufacturer"
                    }
                },
                new CarModel
                {
                    Model = "OtherModel",
                    Manufacturer = manufacturer
                });

            context.SaveChanges();

            var modelsService = new ModelsService(new EfRepository<CarModel>(context));

            var serviceModel = new CarModelServiceModel
            {
                Model = testName,
                ManufacturerId = manufacturer.Id
            };

            // Act
            var result = await modelsService.ExistsAsync(serviceModel);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region GetAllByManufacturerAsync_Tests

        [Fact]
        public async Task GetAllByManufacturerAsync_WithData_WorksCorrectly()
        {
            // Arrange
            var expectedResults = new[] { "Model1", "Model2", "Model3" }
                .OrderBy(m => m)
                .ToArray();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var manufacturer = new CarManufacturer
            {
                Name = "TestManufacturer"
            };

            foreach (var model in expectedResults)
            {
                context.CarModels.Add(new CarModel
                {
                    Model = model,
                    Manufacturer = manufacturer
                });
            }

            context.CarModels.Add(new CarModel
            {
                Model = "OtherModel",
                Manufacturer = new CarManufacturer
                {
                    Name = "OtherManufacturer"
                }
            });

            context.SaveChanges();

            var modelsService = new ModelsService(new EfRepository<CarModel>(context));

            // Act
            var actualResults = (await modelsService.GetAllByManufacturerAsync(manufacturer.Id))
                .Select(m => m.Model)
                .OrderBy(m => m)
                .ToArray();

            // Assert
            Assert.Equal(expectedResults, actualResults);
        }

        [Fact]
        public async Task GetAllByManufacturerAsync_WithNullManufacturerId_ReturnsNull()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarModels.Add(new CarModel
            {
                Model = "OtherModel",
                Manufacturer = new CarManufacturer
                {
                    Name = "OtherManufacturer"
                }
            });

            context.SaveChanges();

            var modelsService = new ModelsService(new EfRepository<CarModel>(context));

            // Act
            var result = await modelsService.GetAllByManufacturerAsync(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllByManufacturerAsync_WithNonExistentManufacturer_ReturnsEmptyEnumerable()
        {
            // Arrange
            var testId = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var modelsService = new ModelsService(new EfRepository<CarModel>(context));

            // Act
            var actualResult = (await modelsService.GetAllByManufacturerAsync(testId)).Count();

            // Assert
            const int expectedResult = 0;
            Assert.Equal(expectedResult, actualResult);
        }

        #endregion
    }
}
