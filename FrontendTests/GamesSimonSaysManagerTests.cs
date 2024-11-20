
using Frontend.Games.SimonSays;

namespace FrontendTests
{
    public class SimonSaysManagerTests
    {
        private readonly SimonSaysManager _manager;

        public SimonSaysManagerTests()
        {
            _manager = new SimonSaysManager();
        }

        [Fact]
        public async Task StartNewGame_InitializesGameCorrectly()
        {
            await _manager.StartNewGame();

            Assert.False(_manager.GameOver);
            Assert.Equal(0, _manager.Level);
            Assert.Empty(_manager.PlayerInput);
            Assert.Single(_manager.Sequence); // A new sequence should be generated
        }

        [Fact]
        public async Task HandleTileClick_AddsToPlayerInputAndHandlesCorrectly()
        {
            await _manager.StartNewGame();
            var correctTile = _manager.Sequence[0] - 1;

            await _manager.HandleTileClick(correctTile);

            Assert.False(_manager.GameOver);

            await _manager.StartNewGame();
            var incorrectTile = _manager.Sequence[0];
            await _manager.HandleTileClick(incorrectTile);

            Assert.Single(_manager.PlayerInput);
            Assert.Equal(incorrectTile + 1, _manager.PlayerInput[0]);
        }

        [Fact]
        public async Task HandleTileClick_EndsGameOnIncorrectInput()
        {
            await _manager.StartNewGame();
            var incorrectTile = (_manager.Sequence[0] % 9) + 1; // Ensure it’s a different tile

            await _manager.HandleTileClick(incorrectTile - 1);

            Assert.True(_manager.GameOver);
        }

        [Fact]
        public async Task HandleTileClick_ProgressesToNextLevel_WhenSequenceIsCorrect()
        {
            await _manager.StartNewGame();
            var sequenceCopy = new List<int>(_manager.Sequence);

            foreach (var tile in sequenceCopy)
            {
                await _manager.HandleTileClick(tile - 1);
            }

            Assert.Equal(1, _manager.Level);
            Assert.Empty(_manager.PlayerInput);
            Assert.Equal(2, _manager.Sequence.Count); // Sequence grows
        }

        [Fact]
        public void SerializedStatistics_ReturnsCorrectJson()
        {
            _manager.SetHighScore(5);

            var json = _manager.SerializedStatistics;

            Assert.Contains("\"HighScore\":5", json);
        }

        [Fact]
        public void LoadStatisticsFromJSON_UpdatesHighScoreCorrectly()
        {
            string json = "{\"HighScore\":10}";

            _manager.LoadStatisticsFromJSON(json);

            Assert.Equal(10, _manager.HighScore);
        }

        [Fact]
        public void LoadStatisticsFromJSON_DoesNothing_WithInvalidJson()
        {
            string invalidJson = "{\"InvalidKey\":15}";

            _manager.LoadStatisticsFromJSON(invalidJson);

            Assert.Equal(0, _manager.HighScore); // Default value
        }

        [Fact]
        public void SetHighScore_UpdatesHighScore_IfHigher()
        {
            var result = _manager.SetHighScore(15);

            Assert.True(result);
            Assert.Equal(15, _manager.HighScore);
        }

        [Fact]
        public void SetHighScore_DoesNotUpdateHighScore_IfLower()
        {
            _manager.SetHighScore(20);

            var result = _manager.SetHighScore(10);

            Assert.False(result);
            Assert.Equal(20, _manager.HighScore); // Unchanged
        }

        [Fact]
        public void Buttons_AreInitializedCorrectly()
        {
            Assert.NotNull(_manager.Buttons);
            Assert.Equal(9, _manager.Buttons.Count);
        }
    }
}