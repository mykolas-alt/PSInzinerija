using Moq;
using Moq.Protected;
using System.Net;
using Microsoft.Extensions.Logging;
using Frontend.Services;
using PSInzinerija1.Shared.Data.Models.Stats;
using Shared.Enums;

namespace Frontend.Tests
{
    public class StatsAPIServiceTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<ILogger<StatsAPIService<GameStats>>> _mockLogger;
        private readonly StatsAPIService<GameStats> _service;

        public StatsAPIServiceTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<StatsAPIService<GameStats>>>();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

            _service = new StatsAPIService<GameStats>(_mockHttpClientFactory.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetStatsAsync_ReturnsNull_WhenRequestFails()
        {
            var game = AvailableGames.SimonSays;

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

            var result = await _service.GetStatsAsync(game);

            Assert.Null(result);
        }
    }
}
