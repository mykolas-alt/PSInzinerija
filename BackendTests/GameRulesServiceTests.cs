using Backend.Services;
using Shared.Data.Models;

namespace BackendTests.Services
{
    public class GameRulesServiceTests
    {
        private readonly GameRulesService _service = new();

        [Fact]
        public async Task GetGameRulesAsync_ReturnsDefaultGameInfo_WhenRulesFileDoesNotExist()
        {
            // Arrange
            string gameRulesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "GameRules");
            string filePath = Path.Combine(gameRulesDirectory, "SimonSaysRules.txt");

            // Ensure the file does not exist
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Act
            GameInfo gameInfo = await _service.GetGameRulesAsync();

            // Assert
            Assert.NotNull(gameInfo);
            Assert.Equal("Simon Says", gameInfo.GameName);
            Assert.Equal(new DateTime(2024, 9, 27), gameInfo.ReleaseDate);
            Assert.Equal(string.Empty, gameInfo.Rules);
        }

        [Fact]
        public async Task GetGameRulesAsync_ReturnsGameInfoWithRules_WhenRulesFileExists()
        {
            // Arrange
            string gameRulesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "GameRules");
            string filePath = Path.Combine(gameRulesDirectory, "SimonSaysRules.txt");

            // Ensure the directory exists
            Directory.CreateDirectory(gameRulesDirectory);

            // Write rules content to the file
            string rulesContent = "These are the rules for Simon Says.";
            await File.WriteAllTextAsync(filePath, rulesContent);

            try
            {
                // Act
                GameInfo gameInfo = await _service.GetGameRulesAsync();

                // Assert
                Assert.NotNull(gameInfo);
                Assert.Equal("Simon Says", gameInfo.GameName);
                Assert.Equal(new DateTime(2024, 9, 27), gameInfo.ReleaseDate);
                Assert.Equal(rulesContent, gameInfo.Rules);
            }
            finally
            {
                // Clean up the file
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Fact]
        public async Task GetGameRulesAsync_HandlesEmptyFileGracefully()
        {
            // Arrange
            string gameRulesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "GameRules");
            string filePath = Path.Combine(gameRulesDirectory, "SimonSaysRules.txt");

            // Ensure the directory exists
            Directory.CreateDirectory(gameRulesDirectory);

            // Create an empty rules file
            await File.WriteAllTextAsync(filePath, string.Empty);

            try
            {
                // Act
                GameInfo gameInfo = await _service.GetGameRulesAsync();

                // Assert
                Assert.NotNull(gameInfo);
                Assert.Equal("Simon Says", gameInfo.GameName);
                Assert.Equal(new DateTime(2024, 9, 27), gameInfo.ReleaseDate);
                Assert.Equal(string.Empty, gameInfo.Rules);
            }
            finally
            {
                // Clean up the file
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }
    }
}
