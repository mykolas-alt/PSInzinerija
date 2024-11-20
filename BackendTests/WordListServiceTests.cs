
using Backend.Services;


public class WordListServiceTests
{
    private readonly WordListService _wordListService;

    public WordListServiceTests()
    {
        _wordListService = new WordListService();
    }

    [Fact]
    public async Task GetWordsFromFileAsync_WhenFileExists_ShouldReturnWords()
    {
        var filePath = "testfile.txt";
        var fileContent = "apple banana, cherry; apple";
        File.WriteAllText(filePath, fileContent);
        var result = await _wordListService.GetWordsFromFileAsync(filePath);
        Assert.NotNull(result);
        Assert.Contains("apple", result);
        Assert.Contains("banana", result);
        Assert.Contains("cherry", result);
        Assert.Equal(3, result.Count);
        File.Delete(filePath);
    }

    [Fact]
    public async Task GetWordsFromFileAsync_WhenFileDoesNotExist_ShouldReturnNull()
    {
        var filePath = "nonexistentfile.txt";
        var result = await _wordListService.GetWordsFromFileAsync(filePath);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetWordsFromFileAsync_WhenFileIsEmpty_ShouldReturnEmptyList()
    {
        var filePath = "emptyfile.txt";
        File.WriteAllText(filePath, string.Empty);

        var result = await _wordListService.GetWordsFromFileAsync(filePath);

        Assert.NotNull(result);
        Assert.Empty(result);
        File.Delete(filePath);
    }
}
