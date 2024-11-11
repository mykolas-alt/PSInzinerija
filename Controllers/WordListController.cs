using Microsoft.AspNetCore.Mvc;

using PSInzinerija1.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using PSInzinerija1.Exceptions;

namespace PSInzinerija1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordListController : ControllerBase
    {
        private readonly WordListService _wordListService;

        public WordListController(WordListService wordListService)
        {
            _wordListService = wordListService;
        }

        /// <summary>
        /// Fetches unique words from a local file.
        /// </summary>
        /// <param name="filePath">Path to the file containing words</param>
        /// <returns>List of unique words</returns>
        [HttpGet("words")]
        public async Task<ActionResult<IEnumerable<string>>> GetWordsFromFileAsync([FromQuery] string filePath)
        {
            try
            {
                var words = await _wordListService.GetWordsFromFileAsync(filePath);

                if (words.Count == 0)
                {
                    return NotFound("File is empty or not found.");
                }

                return Ok(words);
            }
            catch (WordListLoadException ex)
            {
                return BadRequest($"Error loading words from file: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
