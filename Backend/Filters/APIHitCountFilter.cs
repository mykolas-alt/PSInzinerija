using Backend.Services;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Backend.Filters
{
    public class APIHitCountFilter : IActionFilter
    {
        private readonly APITrackingService _hitCounterService;

        public APIHitCountFilter(APITrackingService hitCounterService)
        {
            _hitCounterService = hitCounterService ?? throw new ArgumentNullException(nameof(hitCounterService));
        }

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
