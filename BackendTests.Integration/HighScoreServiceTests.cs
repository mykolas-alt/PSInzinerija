
using System.Net;
using System.Net.Http.Json;
using System.Text;

using Backend.Data.ApplicationDbContext;
using Backend.Data.Models;
using Backend.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using Shared.Enums;

namespace BackendTests.Integration
{
    public class HighScoreServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly HighScoreService _service;
        private readonly Mock<ILogger<HighScoreService>> _loggerMock;

        public HighScoreServiceTests()
        {
            // Configure in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("HighScoreTestDb")
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            // Mock dependencies
            _loggerMock = new Mock<ILogger<HighScoreService>>();

            // Initialize the service
            _service = new HighScoreService(_context, _loggerMock.Object);

            // Seed test data
            SeedDatabase();
        }

        private Mock<UserManager<User>> MockUserManager()
        {
            return new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null, null, null, null, null, null, null, null);
        }

        private void SeedDatabase()
        {
            _context.Users.Add(new User { Id = "User1", UserName = "Player1" });
            _context.HighScores.Add(new HighScoresEntry
            {
                Id = "User1",
                GameId = AvailableGames.VisualMemory,
                HighScore = 100,
                RecordDate = DateTime.UtcNow
            });

            _context.SaveChanges();
        }

        [Fact]
        public async Task GetGameHighScoresAsync_ReturnsLeaderboardEntries_WhenGameExists()
        {
            // Act
            var result = await _service.GetGameHighScoresAsync(AvailableGames.VisualMemory);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Player1", result.First().UserName);
            Assert.Equal(100, result.First().HighScore);
        }

        [Fact]
        public async Task GetAllHighScoresAsync_ReturnsAllHighScores()
        {
            // Act
            var result = await _service.GetAllHighScoresAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Player1", result.First().UserName);
            Assert.Equal(100, result.First().HighScore);
        }
        [Fact]
        public async Task GetUserHighScoreAsync_ReturnsHighScore_WhenUserExists()
        {
            // Act
            var result = await _service.GetUserHighScoreAsync(AvailableGames.VisualMemory, "User1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("User1", result.Id);
            Assert.Equal(100, result.HighScore);
        }

        [Fact]
        public async Task DeleteUserHighScoreAsync_RemovesHighScore_WhenEntryExists()
        {
            // Act
            var success = await _service.DeleteUserHighScoreAsync(AvailableGames.VisualMemory, "User1");

            // Assert
            Assert.True(success);
            Assert.False(_context.HighScores.Any(h => h.Id == "User1" && h.GameId == AvailableGames.VisualMemory));
        }

        [Fact]
        public async Task PutUserHighScoreAsync_UpdatesHighScore_WhenHigherScoreIsProvided()
        {
            // Act
            var success = await _service.PutUserHighScoreAsync(AvailableGames.VisualMemory, 200, "User2");

            // Assert
            Assert.True(success);
            var updatedEntry = _context.HighScores.FirstOrDefault(h => h.Id == "User2" && h.GameId == AvailableGames.VisualMemory);
            Assert.NotNull(updatedEntry);
            Assert.Equal(200, updatedEntry.HighScore);
        }

        [Fact]
        public void EntryExists_ReturnsTrue_WhenEntryExists()
        {
            // Act
            var exists = _context.HighScores.Any(h => h.Id == "User1" && h.GameId == AvailableGames.VisualMemory);

            // Assert
            Assert.True(exists);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}