using Microsoft.AspNetCore.Mvc.Filters;

using PSInzinerija1.Services;

namespace PSInzinerija1.Filters
{
    public class APIHitCountFilter(APITrackingService hitCounterService) : IActionFilter
    {
        private readonly APITrackingService _hitCounterService = hitCounterService;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            string endpointName = context.HttpContext.Request.Path;

            _hitCounterService.IncreaseAPIHitCount(endpointName);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No action needed after the method executes
        }
    }
}
