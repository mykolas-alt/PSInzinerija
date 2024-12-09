using System.Net;
using System.Net.Http.Json;
using PSInzinerija1.Shared.Data.Models.Stats;
using Shared.Enums;

namespace BackendTests.Integration
{
    public class GameStatsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public GameStatsControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetStats_ReturnsOk_WhenStatsExist()
        {
            // Arrange
            var client = _factory.CreateClient();
            var game = AvailableGames.VisualMemory;

            var user = "mock-user-id";

            var validStats = new VisualMemoryStats
            {
                RecentScore = 100,
                GameMistakes = 5
            };

            var saveResponse = await client.PostAsJsonAsync($"api/GameStats/{game}/stats", validStats);
            Assert.Equal(HttpStatusCode.OK, saveResponse.StatusCode);

            // Act
            var response = await client.GetAsync($"api/GameStats/{game}/stats");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stats = await response.Content.ReadFromJsonAsync<VisualMemoryStats>();
            Assert.NotNull(stats);
            Assert.Equal(validStats.RecentScore, stats.RecentScore);
            Assert.Equal(validStats.GameMistakes, stats.GameMistakes);
        }

        [Fact]
        public async Task GetStats_ReturnsBadRequest_WhenGameIsInvalid()
        {
            // Arrange
            var client = _factory.CreateClient();
            var invalidGame = "InvalidGame"; // An invalid game name, not in the enum

            // Act
            var response = await client.GetAsync($"api/GameStats/{invalidGame}/stats");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SaveStats_ReturnsOk_WhenStatsAreValid()
        {
            // Arrange
            var client = _factory.CreateClient();
            var game = AvailableGames.VisualMemory;

            var validStats = new VisualMemoryStats
            {
                RecentScore = 150,
                GameMistakes = 3
            };

            // Act
            var response = await client.PostAsJsonAsync($"api/GameStats/{game}/stats", validStats);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task SaveStats_ReturnsBadRequest_WhenGameIsInvalid()
        {
            // Arrange
            var client = _factory.CreateClient();
            var invalidGame = "InvalidGame"; // An invalid game name, not in the enum

            var validStats = new VisualMemoryStats
            {
                RecentScore = 120,
                GameMistakes = 2
            };

            // Act
            var response = await client.PostAsJsonAsync($"api/GameStats/{invalidGame}/stats", validStats);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetStats_ReturnsOk_WhenSimonSaysStatsExist()
        {
            // Arrange
            var client = _factory.CreateClient();
            var game = AvailableGames.SimonSays;

            var validStats = new SimonSaysStats
            {
                RecentScore = 250,
                TimePlayed = TimeSpan.FromMinutes(12)
            };

            var saveResponse = await client.PostAsJsonAsync($"api/GameStats/{game}/stats", validStats);
            Assert.Equal(HttpStatusCode.OK, saveResponse.StatusCode);

            // Act
            var response = await client.GetAsync($"api/GameStats/{game}/stats");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stats = await response.Content.ReadFromJsonAsync<SimonSaysStats>();
            Assert.NotNull(stats);
            Assert.Equal(validStats.RecentScore, stats.RecentScore);
            Assert.Equal(validStats.TimePlayed, stats.TimePlayed);
        }
        
        [Fact]
        public async Task SaveStats_ReturnsOk_WhenSimonSaysStatsAreValid()
        {
            // Arrange
            var client = _factory.CreateClient();
            var game = AvailableGames.SimonSays;

            var validStats = new SimonSaysStats
            {
                RecentScore = 200,
                TimePlayed = TimeSpan.FromMinutes(10)
            };

            // Act
            var response = await client.PostAsJsonAsync($"api/GameStats/{game}/stats", validStats);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
