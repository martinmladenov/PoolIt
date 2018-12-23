namespace PoolIt.Services.Tests
{
    using System;
    using System.Threading.Tasks;
    using Data;
    using Data.Repository;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using PoolIt.Models;
    using Xunit;

    public class ConversationsServiceTests : BaseTests
    {
        #region GetAsync_Tests

        [Fact]
        public async Task GetAsync_WithCorrectId_WorksCorrectly()
        {
            // Arrange
            var conversation = new Conversation();

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.Conversations.AddRange(
                conversation,
                 new Conversation(),
                 new Conversation()
                );

            context.SaveChanges();

            var conversationsService = new ConversationsService(new EfRepository<Conversation>(context), null, null);

            // Act
            var result = await conversationsService.GetAsync(conversation.Id);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAsync_WithNullId_ReturnsNull()
        {
            // Arrange
            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.Conversations.AddRange(
                new Conversation(),
                new Conversation()
            );

            context.SaveChanges();

            var conversationsService = new ConversationsService(new EfRepository<Conversation>(context), null, null);

            // Act
            var result = await conversationsService.GetAsync(null);

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

            context.Conversations.AddRange(
                new Conversation(),
                new Conversation()
            );

            context.SaveChanges();

            var conversationsService = new ConversationsService(new EfRepository<Conversation>(context), null, null);

            // Act
            var result = await conversationsService.GetAsync(testId);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region SendMessageAsync_Tests

        [Fact]
        public async Task SendMessageAsync_WithValidModel_WorksCorrectly()
        {
            const string expectedMessageContent = "Test Message Content";
            var expectedSentOn = DateTime.UtcNow;

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var conversation = new Conversation();

            context.Conversations.Add(conversation);

            var author = new PoolItUser
            {
                UserName = "TestUser"
            };

            context.Users.Add(author);

            context.SaveChanges();

            var conversationsService = new ConversationsService(
                new EfRepository<Conversation>(context),
                new EfRepository<PoolItUser>(context),
                new EfRepository<Message>(context)
            );

            // Act
            var result = await conversationsService.SendMessageAsync(new MessageServiceModel
            {
                ConversationId = conversation.Id,
                Author = new PoolItUserServiceModel
                {
                    UserName = author.UserName
                },
                SentOn = expectedSentOn,
                Content = expectedMessageContent
            });

            // Assert
            var dbMessage = await context.Messages.SingleAsync(m => m.Id == result.Id);

            Assert.Equal(expectedMessageContent, dbMessage.Content);
            Assert.Equal(expectedSentOn, dbMessage.SentOn);
        }

        [Fact]
        public async Task SendMessageAsync_WithInvalidModel_ReturnsNull()
        {
            var testMessageContent = string.Empty;
            var testSentOn = DateTime.UtcNow;

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var conversation = new Conversation();

            context.Conversations.Add(conversation);

            var author = new PoolItUser
            {
                UserName = "TestUser"
            };

            context.Users.Add(author);

            context.SaveChanges();

            var conversationsService = new ConversationsService(
                new EfRepository<Conversation>(context),
                new EfRepository<PoolItUser>(context),
                new EfRepository<Message>(context)
            );

            // Act
            var result = await conversationsService.SendMessageAsync(new MessageServiceModel
            {
                ConversationId = conversation.Id,
                Author = new PoolItUserServiceModel
                {
                    UserName = author.UserName
                },
                SentOn = testSentOn,
                Content = testMessageContent
            });

            // Assert
            Assert.Null(result);

            var dbCount = await context.Messages.CountAsync();
            Assert.Equal(0, dbCount);
        }

        [Fact]
        public async Task SendMessageAsync_WithNonExistentAuthor_ReturnsNull()
        {
            const string testMessageContent = "Test Message Content";
            var testSentOn = DateTime.UtcNow;

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var conversation = new Conversation();

            context.Conversations.Add(conversation);

            context.SaveChanges();

            var conversationsService = new ConversationsService(
                new EfRepository<Conversation>(context),
                new EfRepository<PoolItUser>(context),
                new EfRepository<Message>(context)
            );

            // Act
            var result = await conversationsService.SendMessageAsync(new MessageServiceModel
            {
                ConversationId = conversation.Id,
                Author = new PoolItUserServiceModel
                {
                    UserName = "NonExistentUser"
                },
                SentOn = testSentOn,
                Content = testMessageContent
            });

            // Assert
            Assert.Null(result);

            var dbCount = await context.Messages.CountAsync();
            Assert.Equal(0, dbCount);
        }

        [Fact]
        public async Task SendMessageAsync_WithNonExistentConversation_ReturnsNull()
        {
            const string testMessageContent = "Test Message Content";
            var testSentOn = DateTime.UtcNow;

            var context = new PoolItDbContext(new DbContextOptionsBuilder<PoolItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var author = new PoolItUser
            {
                UserName = "TestUser"
            };

            context.Users.Add(author);

            context.SaveChanges();

            var conversationsService = new ConversationsService(
                new EfRepository<Conversation>(context),
                new EfRepository<PoolItUser>(context),
                new EfRepository<Message>(context)
            );

            // Act
            var result = await conversationsService.SendMessageAsync(new MessageServiceModel
            {
                ConversationId = Guid.NewGuid().ToString(),
                Author = new PoolItUserServiceModel
                {
                    UserName = author.UserName
                },
                SentOn = testSentOn,
                Content = testMessageContent
            });

            // Assert
            Assert.Null(result);

            var dbCount = await context.Messages.CountAsync();
            Assert.Equal(0, dbCount);
        }

        #endregion
    }
}
