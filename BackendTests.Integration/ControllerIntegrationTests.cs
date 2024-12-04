using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;

using Backend.Controllers;
using Backend.Data.ApplicationDbContext;
using Backend.Data.Models;
using Backend.Services;

using BackendTests.Integration.Helpers;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using Shared.Data.Models;
using Shared.Enums;

namespace BackendTests.Integration
{
    public class ControllerIntegrationTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory = factory;

        [Fact]
        public async Task GetGameHighScoresAsync_ReturnsHighScores_WhenGameExists()
        {
            // Arrange
            var client = _factory.CreateClient();
            using var scope = _factory.Services.CreateScope();
            HighScoresEntry highScoresEntry = new()
            {
                Id = "mock-user-id-1234",
                HighScore = 3,
                GameId = AvailableGames.SimonSays,
                RecordDate = DateTime.UtcNow,
            };
            var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbcontext.HighScores.Add(highScoresEntry);
            dbcontext.SaveChanges();

            // Act
            var response = await client.GetAsync($"api/HighScores/{AvailableGames.SimonSays}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var highScores = await response.Content.ReadFromJsonAsync<LeaderboardEntry>();
            Assert.NotNull(highScores);
        }
    }
}