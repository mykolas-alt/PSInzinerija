using Backend.Controllers;

namespace BackendTests.Integration
{
    public class GameRulesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public GameRulesControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public void GetRules_ThrowsArgumentNullException_WhenGameRulesServiceIsNull()
        {
            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => new GameRulesController(null));
        }
    }
}
