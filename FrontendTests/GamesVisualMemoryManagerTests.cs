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
    }
}