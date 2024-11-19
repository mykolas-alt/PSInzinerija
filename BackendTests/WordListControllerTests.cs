using Microsoft.AspNetCore.Mvc;
using PSInzinerija1.Controllers;
using PSInzinerija1.Services;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

public class WordListControllerTests
{
    private readonly WordListService _wordListService;
    private readonly WordListController _controller;
    private readonly string _testDirectory;

    public WordListControllerTests()
    {
        _wordListService = new WordListService();
        _controller = new WordListController(_wordListService);
        _testDirectory = Path.Combine(Directory.GetCurrentDirectory(), "GameRules");

        if (!Directory.Exists(_testDirectory))
        {
            Directory.CreateDirectory(_testDirectory);
        }
    }

    private async Task CreateFileWithWordsAsync(string fileName, string content)
    {
        var filePath = Path.Combine(_testDirectory, fileName);
        await File.WriteAllTextAsync(filePath, content);
    }


    private void CleanUpFile(string fileName)
    {
        var filePath = Path.Combine(_testDirectory, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public async Task GetWordsFromFileAsync_ShouldReturnOkResult_WhenWordsFound()
    {
        var fileName = "testfile.txt";
        var words = "apple banana cherry";
        await CreateFileWithWordsAsync(fileName, words);
        var result = await _controller.GetWordsFromFileAsync(fileName);
        var okResult = Assert.IsType<ActionResult<IEnumerable<string>>>(result);
        var okObjectResult = Assert.IsType<OkObjectResult>(okResult.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<string>>(okObjectResult.Value);
        Assert.Contains("apple", returnValue);
        Assert.Contains("banana", returnValue);
        Assert.Contains("cherry", returnValue);

        CleanUpFile(fileName);
    }

    [Fact]
    public async Task GetWordsFromFileAsync_ShouldReturnBadRequest_WhenFileNameIsInvalid()
    {
        var fileName = "invalidfile.pdf";

        var result = await _controller.GetWordsFromFileAsync(fileName);

        var actionResult = Assert.IsType<ActionResult<IEnumerable<string>>>(result);
        Assert.IsType<BadRequestResult>(actionResult.Result);
    }

    [Fact]
    public async Task GetWordsFromFileAsync_ShouldReturnNotFound_WhenNoWordsFound()
    {
        var fileName = "emptyfile.txt";
        await CreateFileWithWordsAsync(fileName, string.Empty);
        var result = await _controller.GetWordsFromFileAsync(fileName);
        var actionResult = Assert.IsType<ActionResult<IEnumerable<string>>>(result);
        Assert.IsType<NotFoundResult>(actionResult.Result);

        CleanUpFile(fileName);
    }
}