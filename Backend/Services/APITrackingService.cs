using System.Collections.Concurrent;
using System.Text;

namespace PSInzinerija1.Services
{
    public class APITrackingService
    {
        private readonly ConcurrentDictionary<string, uint> _apiHits = new();
        private readonly DateTime _startTime;
        private readonly ILogger _logger;

        public APITrackingService(IHostApplicationLifetime applicationLifetime, ILogger<APITrackingService> logger)
        {
            _startTime = DateTime.Now;
            applicationLifetime.ApplicationStopped.Register(OnApplicationStopped);
            _logger = logger;
        }

        public uint GetAPIHitCount(string endpoint)
        {
            return _apiHits.TryGetValue(endpoint, out var count) ? count : 0;
        }

        public void IncreaseAPIHitCount(string endpoint)
        {
            var newVal = _apiHits.AddOrUpdate(endpoint, 1, (e, count) => count + 1);
            _logger.LogInformation("{Path} hits: {Count}", endpoint, newVal);
        }

        private void OnApplicationStopped()
        {
            var builder = new StringBuilder($"Total hits from {_startTime} to {DateTime.Now}:");
            foreach (var pair in _apiHits)
            {
                builder.Append($"{Environment.NewLine}{pair.Key}: {pair.Value}");
            }
            _logger.LogInformation(builder.ToString());
        }
    }
}