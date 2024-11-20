using System.Text.Json;

using Frontend.Games.VerbalMemory;

public class VerbalMemoryManagerTests
{
    [Fact]
    public void SerializedStatistics_ReturnsCorrectJSON()
    {
        var manager = new VerbalMemoryManager();
        manager.SetHighScore(10);

        string json = manager.SerializedStatistics;

        var expectedJson = JsonSerializer.Serialize(new { HighScore = 10 });
        Assert.Equal(expectedJson, json);
    }

    [Fact]
    public async Task StartNewGame_ResetsGameState()
    {
        var manager = new VerbalMemoryManager();
        var wordList = new List<string> { "apple", "banana", "cherry" };

        await manager.StartNewGame(wordList);

        Assert.False(manager.GameOver);
        Assert.Equal(0, manager.Score);
        Assert.Equal(0, manager.MistakeCount);
        Assert.Equal(wordList.Count, manager.WordList.Count);
    }

    [Fact]
    public async Task HandleNewButtonClick_IncreasesScoreIfWordIsUnique()
    {
        var manager = new VerbalMemoryManager();
        var wordList = new List<string> { "apple", "banana", "cherry" };
        await manager.StartNewGame(wordList);

        var initialScore = manager.Score;
        manager.CurrentWord = "apple";

        await manager.HandleNewButtonClick();

        Assert.Equal(initialScore + 1, manager.Score);
        Assert.Contains("apple", manager.WordsShown);
    }

    [Fact]
    public async Task HandleNewButtonClick_IncreasesMistakeCountIfWordIsRepeated()
    {
        var manager = new VerbalMemoryManager();
        var wordList = new List<string> { "apple", "banana", "cherry" };
        await manager.StartNewGame(wordList);

        manager.CurrentWord = "apple";
        await manager.HandleNewButtonClick();

        var initialMistakes = manager.MistakeCount;

        manager.CurrentWord = "apple";
        await manager.HandleNewButtonClick();

        Assert.Equal(initialMistakes + 1, manager.MistakeCount);
    }

    [Fact]
    public async Task HandleSeenButtonClick_IncreasesMistakeCountIfWordNotSeen()
    {
        var manager = new VerbalMemoryManager();
        var wordList = new List<string> { "apple", "banana", "cherry" };
        await manager.StartNewGame(wordList);

        manager.CurrentWord = "apple";

        var initialMistakes = manager.MistakeCount;

        await manager.HandleSeenButtonClick();

        Assert.Equal(initialMistakes + 1, manager.MistakeCount);
    }

    [Fact]
    public async Task HandleSeenButtonClick_IncreasesScoreIfWordWasSeen()
    {
        var manager = new VerbalMemoryManager();
        var wordList = new List<string> { "apple", "banana", "cherry" };
        await manager.StartNewGame(wordList);

        manager.CurrentWord = "apple";
        await manager.HandleNewButtonClick();

        var initialScore = manager.Score;

        manager.CurrentWord = "apple";
        await manager.HandleSeenButtonClick();

        Assert.Equal(initialScore + 1, manager.Score);
    }

    [Fact]
    public async Task CheckGameOver_EndsGameWhenMistakeLimitReached()
    {
        var manager = new VerbalMemoryManager();
        var wordList = new List<string> { "apple", "banana", "cherry" };
        await manager.StartNewGame(wordList);

        manager.MistakeCount = 2;
        manager.CurrentWord = "apple";

        await manager.HandleSeenButtonClick();

        Assert.True(manager.MistakeCount == 0); // game restarts after reaching 3 mistakes
    }

    [Fact]
    public void LoadStatisticsFromJSON_UpdatesHighScoreFromValidJSON()
    {
        var manager = new VerbalMemoryManager();
        var json = JsonSerializer.Serialize(new { HighScore = 20 });

        manager.LoadStatisticsFromJSON(json);

        Assert.Equal(20, manager.HighScore);
    }

    [Fact]
    public void SetHighScore_UpdatesHighScoreIfHigher()
    {
        var manager = new VerbalMemoryManager();
        manager.SetHighScore(10);

        Assert.True(manager.SetHighScore(15));
        Assert.Equal(15, manager.HighScore);

        Assert.False(manager.SetHighScore(5));
        Assert.Equal(15, manager.HighScore);
    }

    [Fact]
    public async Task StartNewGame_EnsuresRandomWordIsDisplayed()
    {
        var manager = new VerbalMemoryManager();
        var wordList = new List<string> { "apple", "banana", "cherry" };

        await manager.StartNewGame(wordList);

        Assert.NotNull(manager.CurrentWord);
        Assert.Contains(manager.CurrentWord, wordList);
    }
}
