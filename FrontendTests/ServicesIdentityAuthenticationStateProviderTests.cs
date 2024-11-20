using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;
using PSInzinerija1.Shared.Data.Models;
using Frontend.Services;
using Shared.Data.Models;

namespace FrontendTests
{
    public class IdentityAuthenticationStateProviderTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<ILogger<IdentityAuthenticationStateProvider>> _mockLogger;
        private readonly Mock<HttpMessageHandler> _mockHandler;

        private readonly IdentityAuthenticationStateProvider _authStateProvider;

        public IdentityAuthenticationStateProviderTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<IdentityAuthenticationStateProvider>>();
            _mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            var httpClient = new HttpClient(_mockHandler.Object)
            {
                BaseAddress = new Uri("http://localhost") // Example base URL
            };

            _mockHttpClientFactory
                .Setup(factory => factory.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            _authStateProvider = new IdentityAuthenticationStateProvider(_mockHttpClientFactory.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAuthenticationStateAsync_ReturnsAuthenticatedState_WhenUserInfoIsValid()
        {
            var userInfo = new UserInfo("testuser@gmail.com", "testuser");

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(userInfo)
            };

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().EndsWith("user/info")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            var result = await _authStateProvider.GetAuthenticationStateAsync();

            Assert.NotNull(result);
            Assert.True(result.User.Identity?.IsAuthenticated);
            Assert.Equal("testuser", result.User.Identity?.Name);
            Assert.Contains(result.User.Claims, claim => claim.Type == ClaimTypes.Email && claim.Value == "testuser@gmail.com");
        }
    }
}