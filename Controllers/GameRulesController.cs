using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class GameRulesController : ControllerBase
{
    [HttpGet("stream")]
    public IActionResult GetGameRulesAsStream()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/GameRules/SimonSaysRules.txt");

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("Game rules file not found.");
        }

        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return File(stream, "text/plain");
    }
}
