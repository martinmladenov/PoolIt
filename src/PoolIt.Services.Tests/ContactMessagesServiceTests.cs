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

    public class ContactMessagesServiceTests : BaseTests
    {
        #region CreateAsync_Tests

        [Fact]
        public async Task CreateAsync_WithoutUser_WorksCorrectly()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var contactMessagesService = new ContactMessagesService(new EfRepository<ContactMessage>(context), new EfRepository<PoolItUser>(context));

            var serviceModel = new ContactMessageServiceModel
            {
                Email = "testemail@example.com",
                FullName = "Test Name",
                Subject = "Test Subject",
                Message = "Test Message"
            };

            // Act
            var result = await contactMessagesService.CreateAsync(serviceModel);

            // Assert
            Assert.True(result);

            var dbModel = await context.ContactMessages.SingleOrDefaultAsync();

            Assert.NotNull(dbModel);

            Assert.Equal(serviceModel.Email, dbModel.Email);
            Assert.Equal(serviceModel.FullName, dbModel.FullName);
        }

        [Fact]
        public async Task CreateAsync_WithUser_WorksCorrectly()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var user = new PoolItUser
            {
                UserName = "testuser@example.com",
                Email = "testuser@example.com",
                FirstName = "Test",
                LastName = "User"
            };

            await context.Users.AddAsync(user);

            await context.SaveChangesAsync();

            var contactMessagesService = new ContactMessagesService(new EfRepository<ContactMessage>(context), new EfRepository<PoolItUser>(context));

            var serviceModel = new ContactMessageServiceModel
            {
                Email = "otheremail@example.com",
                FullName = "Other Name",
                Subject = "Test Subject",
                Message = "Test Message",
                UserId = user.Id
            };

            // Act
            var result = await contactMessagesService.CreateAsync(serviceModel);

            // Assert
            Assert.True(result);

            var dbModel = await context.ContactMessages.SingleOrDefaultAsync();

            Assert.NotNull(dbModel);

            Assert.Null(dbModel.Email);
            Assert.Null(dbModel.FullName);

            Assert.Equal(user.Id, dbModel.UserId);
        }

        [Fact]
        public async Task CreateAsync_WithNonExistentUser_ReturnsFalse()
        {
            // Arrange
            var testUserId = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var contactMessagesService = new ContactMessagesService(new EfRepository<ContactMessage>(context), new EfRepository<PoolItUser>(context));

            var serviceModel = new ContactMessageServiceModel
            {
                Email = "testemail@example.com",
                FullName = "Test Name",
                Subject = "Test Subject",
                Message = "Test Message",
                UserId = testUserId
            };

            // Act
            var result = await contactMessagesService.CreateAsync(serviceModel);

            // Assert
            Assert.False(result);

            var messageExists = await context.ContactMessages.AnyAsync();

            Assert.False(messageExists);
        }

        [Fact]
        public async Task CreateAsync_WithInvalidModel_ReturnsFalse()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var contactMessagesService = new ContactMessagesService(new EfRepository<ContactMessage>(context), new EfRepository<PoolItUser>(context));

            var serviceModel = new ContactMessageServiceModel
            {
                Email = "testemail@example.com",
                FullName = "Test Name"
            };

            // Act
            var result = await contactMessagesService.CreateAsync(serviceModel);

            // Assert
            Assert.False(result);

            var messageExists = await context.ContactMessages.AnyAsync();

            Assert.False(messageExists);
        }

        #endregion

        #region GetAllAsync_Tests

        [Fact]
        public async Task GetAllAsync_WithMessages_WorksCorrectly()
        {
            // Arrange
            const int expectedCount = 2;

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            await context.ContactMessages.AddRangeAsync(
                new ContactMessage
                {
                    Email = "testemail@example.com",
                    FullName = "Test Name",
                    Subject = "Test Subject",
                    Message = "Test Message"
                },
                new ContactMessage
                {
                    Email = null,
                    FullName = null,
                    Subject = "Test Subject",
                    Message = "Test Message",
                    User = new PoolItUser
                    {
                        UserName = "testuser@example.com",
                        Email = "testuser@example.com",
                        FirstName = "Test",
                        LastName = "User"
                    }
                }
            );

            await context.SaveChangesAsync();

            var contactMessagesService = new ContactMessagesService(new EfRepository<ContactMessage>(context), new EfRepository<PoolItUser>(context));

            // Act
            var actualCount = (await contactMessagesService.GetAllAsync()).Count();

            // Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetAllAsync_WithNoMessages_ReturnsEmptyEnumerable()
        {
            // Arrange
            const int expectedCount = 0;

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var contactMessagesService = new ContactMessagesService(new EfRepository<ContactMessage>(context), new EfRepository<PoolItUser>(context));

            // Act
            var actualCount = (await contactMessagesService.GetAllAsync()).Count();

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

            var message = new ContactMessage
            {
                Email = null,
                FullName = null,
                Subject = "Test Subject",
                Message = "Test Message",
                User = new PoolItUser
                {
                    UserName = "testuser@example.com",
                    Email = "testuser@example.com",
                    FirstName = "Test",
                    LastName = "User"
                }
            };

            await context.ContactMessages.AddAsync(message);

            await context.SaveChangesAsync();

            var contactMessagesService = new ContactMessagesService(new EfRepository<ContactMessage>(context), new EfRepository<PoolItUser>(context));

            // Act
            var result = await contactMessagesService.DeleteAsync(message.Id);

            // Assert
            Assert.True(result);

            var messageExists = await context.ContactMessages.AnyAsync();
            Assert.False(messageExists);
        }

        [Fact]
        public async Task DeleteAsync_WithNullId_ReturnsFalse()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            await context.SaveChangesAsync();

            var contactMessagesService = new ContactMessagesService(new EfRepository<ContactMessage>(context), new EfRepository<PoolItUser>(context));

            // Act
            var result = await contactMessagesService.DeleteAsync(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistentMessage_ReturnsFalse()
        {
            // Arrange
            var testId = Guid.NewGuid().ToString();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            await context.SaveChangesAsync();

            var contactMessagesService = new ContactMessagesService(new EfRepository<ContactMessage>(context), new EfRepository<PoolItUser>(context));

            // Act
            var result = await contactMessagesService.DeleteAsync(testId);

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}
