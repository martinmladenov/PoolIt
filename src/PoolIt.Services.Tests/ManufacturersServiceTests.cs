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

    public class ManufacturersServiceTests : BaseTests
    {
        #region GetAsync_Tests

        [Fact]
        public async Task GetAsync_WithCorrectId_WorksCorrectly()
        {
            // Arrange
            const string testName = "TestName";

            var manufacturer = new CarManufacturer
            {
                Name = testName
            };

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarManufacturers.AddRange(
                manufacturer,
                 new CarManufacturer
                 {
                     Name = "OtherManufacturer1"
                 },
                 new CarManufacturer
                 {
                     Name = "OtherManufacturer2"
                 });

            context.SaveChanges();

            var manufacturersService = new ManufacturersService(new EfRepository<CarManufacturer>(context));

            // Act
            var actualResult = (await manufacturersService.GetAsync(manufacturer.Id)).Name;

            // Assert
            Assert.Equal(testName, actualResult);
        }

        [Fact]
        public async Task GetAsync_WithNullId_ReturnsNull()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarManufacturers.AddRange(
                 new CarManufacturer
                 {
                     Name = "OtherManufacturer1"
                 },
                 new CarManufacturer
                 {
                     Name = "OtherManufacturer2"
                 });

            context.SaveChanges();

            var manufacturersService = new ManufacturersService(new EfRepository<CarManufacturer>(context));

            // Act
            var result = await manufacturersService.GetAsync(null);

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

            context.CarManufacturers.AddRange(
                 new CarManufacturer
                 {
                     Name = "OtherManufacturer1"
                 },
                 new CarManufacturer
                 {
                     Name = "OtherManufacturer2"
                 });

            context.SaveChanges();

            var manufacturersService = new ManufacturersService(new EfRepository<CarManufacturer>(context));

            // Act
            var result = await manufacturersService.GetAsync(testId);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetByNameAsync_Tests

        [Fact]
        public async Task GetByNameAsync_WithCorrectName_WorksCorrectly()
        {
            // Arrange
            const string testName = "TestName";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarManufacturers.AddRange(
                new CarManufacturer
                {
                    Name = testName
                },
                 new CarManufacturer
                 {
                     Name = "OtherManufacturer1"
                 },
                 new CarManufacturer
                 {
                     Name = "OtherManufacturer2"
                 });

            context.SaveChanges();

            var manufacturersService = new ManufacturersService(new EfRepository<CarManufacturer>(context));

            // Act
            var actualResult = (await manufacturersService.GetByNameAsync(testName)).Name;

            // Assert
            Assert.Equal(testName, actualResult);
        }

        [Fact]
        public async Task GetByNameAsync_WithNullId_ReturnsNull()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarManufacturers.AddRange(
                 new CarManufacturer
                 {
                     Name = "OtherManufacturer1"
                 },
                 new CarManufacturer
                 {
                     Name = "OtherManufacturer2"
                 });

            context.SaveChanges();

            var manufacturersService = new ManufacturersService(new EfRepository<CarManufacturer>(context));

            // Act
            var result = await manufacturersService.GetByNameAsync(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByNameAsync_WithNonExistentName_ReturnsNull()
        {
            // Arrange
            const string testName = "NonExistentName";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarManufacturers.AddRange(
                 new CarManufacturer
                 {
                     Name = "OtherManufacturer1"
                 },
                 new CarManufacturer
                 {
                     Name = "OtherManufacturer2"
                 });

            context.SaveChanges();

            var manufacturersService = new ManufacturersService(new EfRepository<CarManufacturer>(context));

            // Act
            var result = await manufacturersService.GetByNameAsync(testName);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetAllAsync_Tests

        [Fact]
        public async Task GetAll_WithData_WorksCorrectly()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarManufacturers.AddRange(
                new CarManufacturer
                {
                    Name = "Manufacturer1"
                },
                new CarManufacturer
                {
                    Name = "Manufacturer2"
                });

            context.SaveChanges();

            var manufacturersService = new ManufacturersService(new EfRepository<CarManufacturer>(context));

            // Act
            var actualResult = (await manufacturersService.GetAllAsync()).Count();

            // Assert
            const int expectedResult = 2;
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async Task GetAll_WithNoData_ReturnsEmptyEnumerable()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var manufacturersService = new ManufacturersService(new EfRepository<CarManufacturer>(context));

            // Act
            var actualResult = (await manufacturersService.GetAllAsync()).Count();

            // Assert
            const int expectedResult = 0;
            Assert.Equal(expectedResult, actualResult);
        }

        #endregion

        #region CreateAsync_Tests

        [Fact]
        public async Task CreateAsync_WithValidModel_WorksCorrectly()
        {
            // Arrange
            const string testName = "TestName";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var manufacturersService = new ManufacturersService(new EfRepository<CarManufacturer>(context));

            var serviceModel = new CarManufacturerServiceModel
            {
                Name = testName
            };

            // Act
            var result = await manufacturersService.CreateAsync(serviceModel);

            // Assert
            Assert.True(result);

            var addedToDb = await context.CarManufacturers.AnyAsync(m => m.Name == testName);
            Assert.True(addedToDb);
        }

        [Fact]
        public async Task CreateAsync_WithInvalidModel_WorksCorrectly()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var manufacturersService = new ManufacturersService(new EfRepository<CarManufacturer>(context));

            var serviceModel = new CarManufacturerServiceModel();

            // Act
            var result = await manufacturersService.CreateAsync(serviceModel);

            // Assert
            Assert.False(result);

            var dbCount = await context.CarManufacturers.CountAsync();
            Assert.Equal(0, dbCount);
        }

        #endregion

        #region DeleteAsync_Tests

        [Fact]
        public async Task DeleteAsync_WithCorrectId_WorksCorrectly()
        {
            // Arrange
            const string testName = "TestName";

            var manufacturer = new CarManufacturer
            {
                Name = testName
            };

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarManufacturers.AddRange(
                manufacturer,
                new CarManufacturer
                {
                    Name = "OtherManufacturer1"
                },
                new CarManufacturer
                {
                    Name = "OtherManufacturer2"
                });

            context.SaveChanges();

            var manufacturersService = new ManufacturersService(new EfRepository<CarManufacturer>(context));

            // Act
            var result = await manufacturersService.DeleteAsync(manufacturer.Id);

            // Assert
            Assert.True(result);

            var existsInDb = await context.CarManufacturers.AnyAsync(m => m.Name == testName);
            Assert.False(existsInDb);

            var dbCount = await context.CarManufacturers.CountAsync();
            Assert.Equal(2, dbCount);
        }

        [Fact]
        public async Task DeleteAsync_WithNullId_ReturnsFalse()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarManufacturers.AddRange(
                new CarManufacturer
                {
                    Name = "OtherManufacturer1"
                },
                new CarManufacturer
                {
                    Name = "OtherManufacturer2"
                });

            context.SaveChanges();

            var manufacturersService = new ManufacturersService(new EfRepository<CarManufacturer>(context));

            // Act
            var result = await manufacturersService.DeleteAsync(null);

            // Assert
            Assert.False(result);

            var dbCount = await context.CarManufacturers.CountAsync();
            Assert.Equal(2, dbCount);
        }

        [Fact]
        public async Task DeleteAsync_WithCarModels_DoesNotDelete()
        {
            // Arrange
            const string testName = "TestName";

            var manufacturer = new CarManufacturer
            {
                Name = testName,
                Models = new List<CarModel>
                {
                    new CarModel
                    {
                        Model = "TestModel"
                    }
                }
            };

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarManufacturers.AddRange(
                manufacturer,
                new CarManufacturer
                {
                    Name = "OtherManufacturer1"
                },
                new CarManufacturer
                {
                    Name = "OtherManufacturer2"
                });

            context.SaveChanges();

            var manufacturersService = new ManufacturersService(new EfRepository<CarManufacturer>(context));

            // Act
            var result = await manufacturersService.DeleteAsync(manufacturer.Id);

            // Assert
            Assert.False(result);

            var existsInDb = await context.CarManufacturers.AnyAsync(m => m.Name == testName);
            Assert.True(existsInDb);

            var dbCount = await context.CarManufacturers.CountAsync();
            Assert.Equal(3, dbCount);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistentId_ReturnsFalse()
        {
            // Arrange
            var testId = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarManufacturers.AddRange(
                new CarManufacturer
                {
                    Name = "OtherManufacturer1"
                },
                new CarManufacturer
                {
                    Name = "OtherManufacturer2"
                });

            context.SaveChanges();

            var manufacturersService = new ManufacturersService(new EfRepository<CarManufacturer>(context));

            // Act
            var result = await manufacturersService.DeleteAsync(testId);

            // Assert
            Assert.False(result);

            var dbCount = await context.CarManufacturers.CountAsync();
            Assert.Equal(2, dbCount);
        }

        #endregion

        #region UpdateAsync_Tests

        [Fact]
        public async Task UpdateAsync_WithValidModel_WorksCorrectly()
        {
            // Arrange
            const string expectedResult = "TestName";

            var manufacturer = new CarManufacturer
            {
                Name = "InitialName"
            };

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarManufacturers.AddRange(
                manufacturer,
                new CarManufacturer
                {
                    Name = "OtherManufacturer1"
                },
                new CarManufacturer
                {
                    Name = "OtherManufacturer2"
                });

            context.SaveChanges();

            var manufacturersService = new ManufacturersService(new EfRepository<CarManufacturer>(context));

            var serviceModel = new CarManufacturerServiceModel
            {
                Id = manufacturer.Id,
                Name = expectedResult
            };

            // Act
            var result = await manufacturersService.UpdateAsync(serviceModel);

            // Assert
            Assert.True(result);

            var actualResult = (await context.CarManufacturers.SingleAsync(m => m.Id == manufacturer.Id)).Name;
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidModel_DoesNotChange()
        {
            // Arrange
            const string expectedResult = "InitialName";

            var manufacturer = new CarManufacturer
            {
                Name = expectedResult
            };

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarManufacturers.AddRange(
                manufacturer,
                new CarManufacturer
                {
                    Name = "OtherManufacturer1"
                },
                new CarManufacturer
                {
                    Name = "OtherManufacturer2"
                });

            context.SaveChanges();

            var manufacturersService = new ManufacturersService(new EfRepository<CarManufacturer>(context));

            var serviceModel = new CarManufacturerServiceModel
            {
                Id = manufacturer.Id,
                Name = "a"
            };

            // Act
            var result = await manufacturersService.UpdateAsync(serviceModel);

            // Assert
            Assert.False(result);

            var actualResult = (await context.CarManufacturers.SingleAsync(m => m.Id == manufacturer.Id)).Name;
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async Task UpdateAsync_WithNullId_ReturnsFalse()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarManufacturers.AddRange(
                new CarManufacturer
                {
                    Name = "OtherManufacturer1"
                },
                new CarManufacturer
                {
                    Name = "OtherManufacturer2"
                });

            context.SaveChanges();

            var manufacturersService = new ManufacturersService(new EfRepository<CarManufacturer>(context));

            var serviceModel = new CarManufacturerServiceModel
            {
                Id = null,
                Name = "TestName"
            };

            // Act
            var result = await manufacturersService.UpdateAsync(serviceModel);

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

            context.CarManufacturers.AddRange(
                new CarManufacturer
                {
                    Name = "OtherManufacturer1"
                },
                new CarManufacturer
                {
                    Name = "OtherManufacturer2"
                });

            context.SaveChanges();

            var manufacturersService = new ManufacturersService(new EfRepository<CarManufacturer>(context));

            var serviceModel = new CarManufacturerServiceModel
            {
                Id = Guid.NewGuid().ToString(),
                Name = "TestName"
            };

            // Act
            var result = await manufacturersService.UpdateAsync(serviceModel);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region ExistsAsync_Tests

        [Fact]
        public async Task ExistsAsync_WithExistentName_ReturnsTrue()
        {
            // Arrange
            const string testName = "TestName";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarManufacturers.AddRange(
                new CarManufacturer
                {
                    Name = testName
                },
                new CarManufacturer
                {
                    Name = "OtherManufacturer"
                });

            context.SaveChanges();

            var manufacturersService = new ManufacturersService(new EfRepository<CarManufacturer>(context));

            // Act
            var result = await manufacturersService.ExistsByNameAsync(testName);

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

            context.CarManufacturers.AddRange(
                new CarManufacturer
                {
                    Name = "Manufacturer1"
                },
                new CarManufacturer
                {
                    Name = "Manufacturer2"
                });

            context.SaveChanges();

            var manufacturersService = new ManufacturersService(new EfRepository<CarManufacturer>(context));

            // Act
            var result = await manufacturersService.ExistsByNameAsync(testName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExistsAsync_WithUppercaseName_ReturnsTrue()
        {
            // Arrange
            const string testName = "TestName";

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.CarManufacturers.AddRange(
                new CarManufacturer
                {
                    Name = testName
                },
                new CarManufacturer
                {
                    Name = "OtherManufacturer"
                });

            context.SaveChanges();

            var manufacturersService = new ManufacturersService(new EfRepository<CarManufacturer>(context));

            // Act
            var result = await manufacturersService.ExistsByNameAsync(testName.ToUpperInvariant());

            // Assert
            Assert.True(result);
        }

        #endregion
    }
}
