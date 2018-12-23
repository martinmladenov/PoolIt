namespace PoolIt.Services.Tests
{
    using System.Collections.Generic;
    using Helpers;
    using Xunit;

    public class RandomStringGeneratorHelperTests : BaseTests
    {
        #region GenerateRandomString_Tests

        [Fact]
        public void GenerateRandomString_ReturnsCorrectLength()
        {
            // Arrange
            const int expectedLength = 5;
            var randomStringGeneratorHelper = new RandomStringGeneratorHelper();

            // Act
            var actualLength = randomStringGeneratorHelper.GenerateRandomString(expectedLength).Length;

            // Assert
            Assert.Equal(expectedLength, actualLength);
        }

        [Fact]
        public void GenerateRandomString_ReturnsRandomResults()
        {
            // Arrange
            const int iterations = 100;
            const int stringLength = 15;

            var randomStringGeneratorHelper = new RandomStringGeneratorHelper();

            var results = new HashSet<string>();

            // Act
            for (int i = 0; i < iterations; i++)
            {
                var result = randomStringGeneratorHelper.GenerateRandomString(stringLength);
                results.Add(result);
            }

            // Assert
            Assert.Equal(iterations, results.Count);
        }

        #endregion
    }
}
