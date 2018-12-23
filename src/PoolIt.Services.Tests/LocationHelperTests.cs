namespace PoolIt.Services.Tests
{
    using System.IO.Abstractions.TestingHelpers;
    using System.Threading.Tasks;
    using Helpers;
    using Xunit;

    public class LocationHelperTests : BaseTests
    {
        private const string TownsFileName = "towns.json";

        #region GetGetTownNameAsync_Tests

        [Fact]
        public async Task GetTownNameAsync_WithCorrectData_WorksCorrectly()
        {
            // Arrange
            const string expectedResult = "TestTown";

            var json =
                $@"[
                  {{
                    ""name"": ""Town1"",
                    ""latitude"": 10.5,
                    ""longitude"": 11.7
                  }},
                  {{
                    ""name"": ""{expectedResult}"",
                    ""latitude"": -1.5,
                    ""longitude"": -2.0004
                  }},
                  {{
                    ""name"": ""Town2"",
                    ""latitude"": 7.7,
                    ""longitude"": 6.1
                  }}
                ]";

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(TownsFileName, new MockFileData(json));

            var locationHelper = new LocationHelper(mockFileSystem);

            // Act
            var actualResult = await locationHelper.GetTownNameAsync(2, 2);

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async Task GetTownNameAsync_WithNoData_ReturnsNull()
        {
            // Arrange
            const string json = "[]";

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(TownsFileName, new MockFileData(json));

            var locationHelper = new LocationHelper(mockFileSystem);

            // Act
            var result = await locationHelper.GetTownNameAsync(2, 2);

            // Assert
            Assert.Null(result);
        }

        #endregion
    }
}
