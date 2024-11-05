using Microsoft.AspNetCore.Mvc;

namespace PSInzinerija1.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected ActionResult HandleResponse<T>(T result, string notFoundMessage = "Resource not found") where T : class, IEnumerable
        {
            return result != null ? Ok(result) : NotFound(notFoundMessage);
        }
    }
}
