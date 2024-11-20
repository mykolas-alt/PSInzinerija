using Backend.Services;

using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordListController : ControllerBase
    {
        private readonly WordListService _wordListService;

        public WordListController(WordListService wordListService)
        {
            _wordListService = wordListService ?? throw new ArgumentNullException(nameof(wordListService));
        }

        [HttpGet("words")]
        public async Task<ActionResult<IEnumerable<string>>> GetWordsFromFileAsync([FromQuery] string fileName)
        {
            if (!fileName.EndsWith(".txt"))
            {
                return BadRequest();
            }

            var filePath = Path.Combine("GameRules/", fileName);
            var words = await _wordListService.GetWordsFromFileAsync(filePath);

            if (words == null || words.Count == 0)
            {
                return NotFound();
            }

            return Ok(words);
        }
    }
}