using Moq;
using Moq.Protected;
using Microsoft.Extensions.Logging;
using Frontend.Services;
using Shared.Enums;

namespace FrontendTests
{
    public class HighScoreAPIServiceTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<ILogger<HighScoreAPIService>> _mockLogger;
        private readonly HighScoreAPIService _service;

        public HighScoreAPIServiceTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<HighScoreAPIService>>();

            // Create a mock HttpClientHandler to mock HttpClient calls
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            // Create HttpClient using the mocked HttpMessageHandler
            var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            // Mock IHttpClientFactory to return the mock HttpClient
            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

            _service = new HighScoreAPIService(_mockHttpClientFactory.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetLeaderboardEntriesAsync_ReturnsEmptyList_WhenErrorOccurs()
        {
            var game = AvailableGames.SimonSays;

            // Mock error response
            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(CreateMockHttpClientWithError());

            var result = await _service.GetLeaderboardEntriesAsync(game);

            Assert.NotNull(result);
            Assert.Empty(result); // Should return an empty list in case of error
        }

        [Fact]
        public async Task GetHighScoreAsync_ReturnsNull_WhenErrorOccurs()
        {
            var game = AvailableGames.SimonSays;

            // Mock error response
            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(CreateMockHttpClientWithError());

            var result = await _service.GetHighScoreAsync(game);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteFromDbAsync_ReturnsFalse_WhenErrorOccurs()
        {
            var game = AvailableGames.SimonSays;

            // Mock error response
            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(CreateMockHttpClientWithError());

            var result = await _service.DeleteFromDbAsync(game);

            Assert.False(result);
        }

        [Fact]
        public async Task SaveHighScoreToDbAsync_ReturnsFalse_WhenErrorOccurs()
        {
            var game = AvailableGames.SimonSays;
            var newHighScore = 250;

            // Mock error response
            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(CreateMockHttpClientWithError());

            var result = await _service.SaveHighScoreToDbAsync(game, newHighScore);

            Assert.False(result);
        }

        // Helper method to create a mock HttpClient with an error response
        private HttpClient CreateMockHttpClientWithError()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Request error"));

            return new HttpClient(mockHttpMessageHandler.Object);
        }
    }
}