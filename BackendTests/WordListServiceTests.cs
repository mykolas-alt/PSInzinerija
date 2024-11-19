using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using PSInzinerija1.Services;

using Xunit;

public class WordListServiceTests
{
    private readonly WordListService _wordListService;

    public WordListServiceTests()
    {
        _wordListService = new WordListService();
    }

    [Fact]
    public async Task GetWordsFromFileAsync_ShouldReturnWords_WhenFileExists()
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
    }

    [Fact]
    public async Task GetWordsFromFileAsync_ShouldReturnNull_WhenFileDoesNotExist()
    {
        var filePath = "nonexistentfile.txt";
        var result = await _wordListService.GetWordsFromFileAsync(filePath);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetWordsFromFileAsync_ShouldReturnEmptyList_WhenFileIsEmpty()
    {
        var filePath = "emptyfile.txt";
        File.WriteAllText(filePath, string.Empty);

        var result = await _wordListService.GetWordsFromFileAsync(filePath);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
