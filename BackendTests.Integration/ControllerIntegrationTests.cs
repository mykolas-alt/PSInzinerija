using System.Net;
using System.Net.Http.Json;
using System.Text;

using Backend.Data.ApplicationDbContext;
using Backend.Data.Models;

using BackendTests.Integration.Helpers;

using Microsoft.Extensions.DependencyInjection;

using Shared.Data.Models;
using Shared.Enums;

namespace BackendTests.Integration
{
    public class ControllerIntegrationTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory = factory;

        [Fact]
        public async Task GetAllHighScoresAsync_ReturnsEmptyList_WhenNoHighScoresExist()
        {
            // Arrange
            var client = _factory.CreateClient();
            _factory.ResetHighScoresTable();

            // Act
            var response = await client.GetAsync($"api/HighScores");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var highScores = await response.Content.ReadFromJsonAsync<IEnumerable<LeaderboardEntry>>();
            Assert.NotNull(highScores);
            Assert.Equal([], highScores);
        }

        [Fact]
        public async Task GetAllHighScoresAsync_ReturnsAllHighScores_WhenHighScoresExist()
        {
            // Arrange
            _factory.ResetHighScoresTable();
            _factory.ResetUsersScoresTable();
            using var scope = _factory.Services.CreateScope();
            var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            List<HighScoresEntry> highScoresEntryList = [
            new()
            {
                Id = "mock-user-id-1234",
                HighScore = 3,
                GameId = AvailableGames.VisualMemory,
                RecordDate = DateTime.UtcNow,
            },
            new()
            {
                Id = "mock-user-id-1234",
                HighScore = 10,
                GameId = AvailableGames.SimonSays,
                RecordDate = DateTime.UtcNow,
            },
            new()
            {
                Id = "mock-user-id-4999",
                HighScore = 6,
                GameId = AvailableGames.SimonSays,
                RecordDate = DateTime.UtcNow,
            },
            new()
            {
                Id = "mock-user-id-3746",
                HighScore = 17,
                GameId = AvailableGames.VerbalMemory,
                RecordDate = DateTime.UtcNow,
            }];
            dbcontext.HighScores.AddRange(highScoresEntryList);
            dbcontext.Users.AddRange([
                new() { Id = "mock-user-id-1234", UserName = "mock-user-id-1234" },
                new() { Id = "mock-user-id-4999", UserName = "mock-user-id-4999" },
                new() { Id = "mock-user-id-3746", UserName = "mock-user-id-3746" }
                ]);
            dbcontext.SaveChanges();
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync($"api/HighScores");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var highScores = await response.Content.ReadFromJsonAsync<IEnumerable<LeaderboardEntry>>();
            Assert.NotNull(highScores);
            var leaderboardList = highScoresEntryList.Select(e => new LeaderboardEntry(e.Id, e.HighScore, e.RecordDate)).OrderByDescending(e => e.HighScore).ToList();
            Assert.Equal(leaderboardList, highScores);
        }

        [Theory]
        [InlineData(AvailableGames.SimonSays)]
        [InlineData(AvailableGames.VerbalMemory)]
        [InlineData(AvailableGames.VisualMemory)]
        public async Task GetGameHighScoresAsync_ReturnsEmptyList_WhenNoHighScoresExist(AvailableGames game)
        {
            // Arrange
            var client = _factory.CreateClient();
            _factory.ResetHighScoresTable();

            // Act
            var response = await client.GetAsync($"api/HighScores/{game}/all");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var highScores = await response.Content.ReadFromJsonAsync<IEnumerable<LeaderboardEntry>>();
            Assert.NotNull(highScores);
            Assert.Equal([], highScores);
        }

        [Theory]
        [InlineData(AvailableGames.SimonSays)]
        [InlineData(AvailableGames.VerbalMemory)]
        [InlineData(AvailableGames.VisualMemory)]
        public async Task GetGameHighScoresAsync_ReturnsListOfGameHighScores_WhenHighScoresExist(AvailableGames game)
        {
            // Arrange
            _factory.ResetHighScoresTable();
            _factory.ResetUsersScoresTable();
            using var scope = _factory.Services.CreateScope();
            var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            List<HighScoresEntry> highScoresEntryList = [
            new()
            {
                Id = "mock-user-id-1234",
                HighScore = 3,
                GameId = game,
                RecordDate = DateTime.UtcNow,
            },
            new()
            {
                Id = "mock-user-id-4999",
                HighScore = 6,
                GameId = game,
                RecordDate = DateTime.UtcNow,
            },
            new()
            {
                Id = "mock-user-id-3746",
                HighScore = 17,
                GameId = game,
                RecordDate = DateTime.UtcNow,
            }];
            dbcontext.HighScores.AddRange(highScoresEntryList);
            dbcontext.Users.AddRange([
                new() { Id = "mock-user-id-1234", UserName = "mock-user-id-1234" },
                new() { Id = "mock-user-id-4999", UserName = "mock-user-id-4999" },
                new() { Id = "mock-user-id-3746", UserName = "mock-user-id-3746" }
                ]);
            dbcontext.SaveChanges();
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync($"api/HighScores/{game}/all");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var highScores = await response.Content.ReadFromJsonAsync<IEnumerable<LeaderboardEntry>>();
            Assert.NotNull(highScores);
            var leaderboardList = highScoresEntryList.Select(e => new LeaderboardEntry(e.Id, e.HighScore, e.RecordDate)).OrderByDescending(e => e.HighScore).ToList();
            Assert.Equal(leaderboardList, highScores);
        }

        [Theory]
        [InlineData(AvailableGames.SimonSays)]
        [InlineData(AvailableGames.VerbalMemory)]
        [InlineData(AvailableGames.VisualMemory)]
        public async Task GetUserHighScoreAsync_ReturnsHighScores_WhenGameExists(AvailableGames game)
        {
            // Arrange
            var client = _factory.CreateClient();
            _factory.ResetHighScoresTable();
            using var scope = _factory.Services.CreateScope();
            var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            HighScoresEntry highScoresEntry = new()
            {
                Id = "mock-user-id-1234",
                HighScore = 3,
                GameId = game,
                RecordDate = DateTime.UtcNow,
            };
            dbcontext.HighScores.Add(highScoresEntry);
            dbcontext.SaveChanges();

            // Act
            var response = await client.GetAsync($"api/HighScores/{game}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var highScores = await response.Content.ReadFromJsonAsync<LeaderboardEntry>();
            Assert.NotNull(highScores);
        }

        [Fact]
        public async Task GetUserHighScoreAsync_ReturnsBadRequest_WhenGameDoesNotExist()
        {
            // Arrange
            var client = _factory.CreateClient();
            var nonExistentGame = "Chess";

            // Act
            var response = await client.GetAsync($"api/HighScores/{nonExistentGame}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData(AvailableGames.SimonSays)]
        [InlineData(AvailableGames.VerbalMemory)]
        [InlineData(AvailableGames.VisualMemory)]
        public async Task GetUserHighScoreAsync_ReturnsNotFound_WhenHighScoreDoesNotExist(AvailableGames game)
        {
            // Arrange
            var client = _factory.CreateClient();
            _factory.ResetHighScoresTable();

            // Act
            var response = await client.GetAsync($"api/HighScores/{game}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData(AvailableGames.SimonSays)]
        [InlineData(AvailableGames.VerbalMemory)]
        [InlineData(AvailableGames.VisualMemory)]
        public async Task PutUserHighScoreAsync_InsertsUserHighScore_WhenNoHighScoreExists(AvailableGames game)
        {
            // Arrange
            var client = _factory.CreateClient();
            _factory.ResetHighScoresTable();
            _factory.ResetUsersScoresTable();

            using var scope = _factory.Services.CreateScope();
            var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbcontext.Users.Add(new() { Id = "mock-user-id-1234", UserName = "mock-user-id-1234" });

            int newHighScore = 5;
            var content = new StringContent(newHighScore.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync($"api/HighScores/{game}", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData(AvailableGames.SimonSays)]
        [InlineData(AvailableGames.VerbalMemory)]
        [InlineData(AvailableGames.VisualMemory)]
        public async Task PutUserHighScoreAsync_UpdatesUserScore_WhenHighScoerExists(AvailableGames game)
        {
            // Arrange
            var client = _factory.CreateClient();
            _factory.ResetHighScoresTable();
            _factory.ResetUsersScoresTable();

            using var scope = _factory.Services.CreateScope();
            var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbcontext.Users.Add(new() { Id = "mock-user-id-1234", UserName = "mock-user-id-1234" });
            dbcontext.HighScores.Add(new() { Id = "mock-user-id-1234", HighScore = 5, GameId = game, RecordDate = DateTime.UtcNow });

            int newHighScore = 11;
            var content = new StringContent(newHighScore.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync($"api/HighScores/{game}", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PutUserHighScoreAsync_ReturnsBadRequest_WhenGameDoesNotExist()
        {
            // Arrange
            var client = _factory.CreateClient();
            var nonExistentGame = "Chess";

            int newHighScore = 11;
            var content = new StringContent(newHighScore.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync($"api/HighScores/{nonExistentGame}", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData(AvailableGames.SimonSays)]
        [InlineData(AvailableGames.VerbalMemory)]
        [InlineData(AvailableGames.VisualMemory)]
        public async Task DeleteUserHighScoreAsync_DeletesHighScore_WhenHighScoreExists(AvailableGames game)
        {
            // Arrange
            var client = _factory.CreateClient();
            _factory.ResetHighScoresTable();
            _factory.ResetUsersScoresTable();

            using var scope = _factory.Services.CreateScope();
            var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbcontext.Users.Add(new() { Id = "mock-user-id-1234", UserName = "mock-user-id-1234" });
            dbcontext.HighScores.Add(new() { Id = "mock-user-id-1234", HighScore = 5, GameId = game, RecordDate = DateTime.UtcNow });
            dbcontext.SaveChanges();

            // Act
            var response = await client.DeleteAsync($"api/HighScores/{game}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Empty(dbcontext.HighScores);
        }

        [Fact]
        public async Task DeleteUserHighScoreAsync_ReturnsBadRequest_WhenGameDoesNotExist()
        {
            // Arrange
            var client = _factory.CreateClient();
            var nonExistentGame = "Chess";

            // Act
            var response = await client.DeleteAsync($"api/HighScores/{nonExistentGame}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}