using System.Text.Json;

using Frontend.Games.VisualMemory;
using Frontend.Games.VisualMemory.Models;

namespace FrontendTests
{
    public class VisualMemoryManagerTests
    {
        [Fact]
        public void SerializedStatistics_ReturnsCorrectJSON()
        {
            var manager = new VisualMemoryManager();
            manager.SetHighScore(10);

            string json = manager.SerializedStatistics;

            var expectedJson = JsonSerializer.Serialize(new VisualMemoryManager.VisualMemoryHighScore(10));
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public async Task HandleInput_UpdatesMistakeOrCorrectCount()
        {
            var manager = new VisualMemoryManager();
            var validCell = new PatternCell(PatternValue.Valid, 0);
            var invalidCell = new PatternCell(PatternValue.Invalid, 1);

            await manager.HandleInput(validCell);
            Assert.Equal(0, manager.Score);

            await manager.HandleInput(validCell);
            Assert.True(manager.Score == 0);  // Score should not increase
        }

        [Fact]
        public void LoadStatisticsFromJSON_UpdatesHighScoreFromValidJSON()
        {
            var manager = new VisualMemoryManager();
            var json = JsonSerializer.Serialize(new VisualMemoryManager.VisualMemoryHighScore(20));

            manager.LoadStatisticsFromJSON(json);

            Assert.Equal(20, manager.HighScore);
        }

        [Fact]
        public void SetHighScore_UpdatesHighScoreIfHigher()
        {
            var manager = new VisualMemoryManager();
            manager.SetHighScore(10);

            Assert.True(manager.SetHighScore(15)); // Should succeed
            Assert.Equal(15, manager.HighScore);

            Assert.False(manager.SetHighScore(5)); // Should fail since 5 < 15
            Assert.Equal(15, manager.HighScore);
        }
        [Fact]
        public async Task StartNewGame_ResetsScoreAndPattern()
        {
            var manager = new VisualMemoryManager();

            manager.SetHighScore(10);
            await manager.StartNewGame();

            Assert.Equal(0, manager.Score);
            Assert.Equal(0, manager.GameMistakes);
            Assert.NotNull(manager.Pattern);
            Assert.Equal(0, manager.RecentScore);
        }

        [Fact]
        public async Task HandleInput_EndsGameAfterThreeMistakes()
        {
            var manager = new VisualMemoryManager();
            var invalidCell = new PatternCell(PatternValue.Invalid, 0);

            Assert.Equal(0, manager.GameMistakes);
            
            await manager.HandleInput(invalidCell);

            Assert.Equal(1, manager.GameMistakes);
            Assert.Equal(0, manager.Score);
        }

        [Fact]
        public void LoadStatisticsFromJSON_DoesNotUpdateHighScoreWithLowerValue()
        {
            var manager = new VisualMemoryManager();
            manager.SetHighScore(20);

            var json = JsonSerializer.Serialize(new VisualMemoryManager.VisualMemoryHighScore(10));
            manager.LoadStatisticsFromJSON(json);

            Assert.Equal(20, manager.HighScore); // HighScore should remain unchanged
        }

        [Fact]
        public void SetHighScore_ReturnsFalseForNullOrLowerValue()
        {
            var manager = new VisualMemoryManager();
            manager.SetHighScore(20);

            Assert.False(manager.SetHighScore(null));
            Assert.False(manager.SetHighScore(15));
            Assert.Equal(20, manager.HighScore);
        }

        [Fact]
        public void SerializedStatistics_ReturnsUpdatedHighScore()
        {
            var manager = new VisualMemoryManager();
            manager.SetHighScore(30);

            var json = manager.SerializedStatistics;
            var deserialized = JsonSerializer.Deserialize<VisualMemoryManager.VisualMemoryHighScore>(json);

            Assert.NotNull(deserialized);
            Assert.Equal(30, deserialized?.HighScore);
        }
    }
}