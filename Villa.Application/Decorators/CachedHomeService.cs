using Microsoft.Extensions.Logging;
using Villla.Application.Dtos;
using Villla.Application.Services.Interface;
using Villla.Application.Services.Interface.Cashing;

namespace Villla.Application.Decorators
{
    public class CachedHomeService : IHomeService
    {
        private readonly IHomeService _inner;
        private readonly ICacheService _cache;
        private readonly ILogger<CachedHomeService> _logger;

        private const string KEY_HOME = "home_villas";

        public CachedHomeService(
            IHomeService inner,
            ICacheService cache,
            ILogger<CachedHomeService> logger)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ================= GET INDEX =================
        public async Task<HomeDto> GetHomeDataAsync(int? villaId, int? scroll)
        {
            try
            {
                var cached = _cache.Get<HomeDto>(KEY_HOME);

                if (cached != null)
                {
                    _logger.LogInformation("Cache HIT - Home Data");
                    return cached;
                }

                _logger.LogInformation("Cache MISS - Home Data");

                var result = await _inner.GetHomeDataAsync(villaId, scroll);

                if (result != null)
                {
                    _cache.Set(KEY_HOME, result, TimeSpan.FromMinutes(10));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error caching GetHomeData");
                throw;
            }
        }

        // ================= FILTER =================
        public async Task<HomeDto> FilterHomeDataAsync(HomeDto dto)
        {
            try
            {
                // ❌ DO NOT CACHE
                // reason: it's UI state + user input transformation
                return await _inner.FilterHomeDataAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FilterHomeData");
                throw;
            }
        }

        // ================= FILTER BY DATE =================
        public async Task<HomeDto> GetVillasByDateAsync(int nights, DateOnly checkInDate)
        {
            try
            {
                // ❌ DO NOT CACHE
                // reason: dynamic availability (bookings + date + logic)
                return await _inner.GetVillasByDateAsync(nights, checkInDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetVillasByDate caching layer");
                throw;
            }
        }
    }
}