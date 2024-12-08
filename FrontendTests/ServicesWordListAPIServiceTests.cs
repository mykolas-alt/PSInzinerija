using System.Net;
using System.Net.Http;
using System.Text.Json;
using Moq;
using Moq.Protected;
using Xunit;
using Frontend.Services;
using Frontend.Exceptions;
using System.Text;

namespace FrontendTests
{
    public class WordListAPIServiceTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;

        public WordListAPIServiceTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        }

        [Fact]
        public async Task GetWordsFromApiAsync_ReturnsWords_WhenResponseIsValid()
        {
            var expectedWords = new List<string> { "apple", "banana", "cherry" };
            var mockHttpClient = CreateMockHttpClientWithResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedWords));

            _mockHttpClientFactory.Setup(x => x.CreateClient("BackendApi")).Returns(mockHttpClient);

            var service = new WordListAPIService(_mockHttpClientFactory.Object);
            var result = await service.GetWordsFromApiAsync("testFile");

            Assert.NotNull(result);
            Assert.Equal(expectedWords, result);
        }

        [Fact]
        public async Task GetWordsFromApiAsync_ThrowsWordListLoadException_OnHttpRequestException()
        {
            var mockHttpClient = CreateMockHttpClientWithError();
            _mockHttpClientFactory.Setup(x => x.CreateClient("BackendApi")).Returns(mockHttpClient);

            var service = new WordListAPIService(_mockHttpClientFactory.Object);

            await Assert.ThrowsAsync<WordListLoadException>(() => service.GetWordsFromApiAsync("testFile"));
        }

        [Fact]
        public async Task GetWordsFromApiAsync_ThrowsWordListLoadException_OnNonSuccessStatusCode()
        {
            // Arrange
            var mockHttpClient = CreateMockHttpClientWithResponse(HttpStatusCode.InternalServerError, string.Empty);
            _mockHttpClientFactory.Setup(x => x.CreateClient("BackendApi")).Returns(mockHttpClient);

            var service = new WordListAPIService(_mockHttpClientFactory.Object);

            await Assert.ThrowsAsync<WordListLoadException>(() => service.GetWordsFromApiAsync("testFile"));
        }

        [Fact]
        public async Task GetWordsFromApiAsync_ThrowsWordListLoadException_OnNullContent()
        {
            // Arrange
            var mockHttpClient = CreateMockHttpClientWithResponse(HttpStatusCode.OK, "null");
            _mockHttpClientFactory.Setup(x => x.CreateClient("BackendApi")).Returns(mockHttpClient);

            var service = new WordListAPIService(_mockHttpClientFactory.Object);

            // Act & Assert
            await Assert.ThrowsAsync<WordListLoadException>(() => service.GetWordsFromApiAsync("testFile"));
        }

        // Helper method: Mock HttpClient for successful response
        private HttpClient CreateMockHttpClientWithResponse(HttpStatusCode statusCode, string content)
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                });

            return new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };
        }
        private HttpClient CreateMockHttpClientWithError()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Simulated HTTP request failure"));

            return new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };
        }
    }
}
