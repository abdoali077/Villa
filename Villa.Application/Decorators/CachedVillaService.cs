using Microsoft.Extensions.Logging;
using Villla.Application.Dtos;
using Villla.Application.Services.Interface;
using Villla.Application.Services.Interface.Cashing;

namespace Villla.Application.Decorators
{
    public class CachedVillaService : IVillaService
    {
        private readonly IVillaService _inner;
        private readonly ICacheService _cache;
        private readonly ILogger<CachedVillaService> _logger;

        private const string KEY_ALL = "villas_all";

        private string GetKey(int id) => $"villa_{id}";

        public CachedVillaService(
            IVillaService inner,
            ICacheService cache,
            ILogger<CachedVillaService> logger)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ================= CREATE =================
        public async Task CreateAsync(VillaDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("CreateAsync called with null dto");
                throw new ArgumentNullException(nameof(dto));
            }

            try
            {
                await _inner.CreateAsync(dto);

                _cache.Remove(KEY_ALL);
                _cache.Remove(GetKey(dto.Id));

                _logger.LogInformation("Villa created and cache invalidated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateAsync for Villa {Name}", dto.Name);
                throw;
            }
        }

        // ================= DELETE =================
        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("DeleteAsync called with invalid id: {Id}", id);
                throw new ArgumentException("Invalid Villa Id", nameof(id));
            }

            try
            {
                await _inner.DeleteAsync(id);

                _cache.Remove(KEY_ALL);
                _cache.Remove(GetKey(id));
                _logger.LogInformation("Villa {Id} deleted and cache invalidated", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Villa {Id}", id);
                throw;
            }
        }

        // ================= GET ALL =================
        public async Task<IEnumerable<VillaDto>> GetAllAsync()
        {
            try
            {
                var cached = _cache.Get<IEnumerable<VillaDto>>(KEY_ALL);

                if (cached != null)
                {
                    _logger.LogInformation("Cache HIT - GetAll Villas");
                    return cached;
                }

                _logger.LogInformation("Cache MISS - GetAll Villas");

                var result = await _inner.GetAllAsync();

                if (result == null)
                    return Enumerable.Empty<VillaDto>();

                _cache.Set(KEY_ALL, result.ToList(), TimeSpan.FromMinutes(5));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync");
                throw;
            }
        }

        // ================= GET BY ID =================
        public async Task<VillaDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("GetByIdAsync called with invalid id: {Id}", id);
                throw new ArgumentException("Invalid Villa Id", nameof(id));
            }

            try
            {
                var key = GetKey(id);

                var cached = _cache.Get<VillaDto>(key);

                if (cached != null)
                {
                    _logger.LogInformation("Cache HIT - Villa {Id}", id);
                    return cached;
                }

                _logger.LogInformation("Cache MISS - Villa {Id}", id);

                var result = await _inner.GetByIdAsync(id);

                if (result != null)
                {
                    _cache.Set(key, result, TimeSpan.FromMinutes(5));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetByIdAsync for Villa {Id}", id);
                throw;
            }
        }

        // ================= UPDATE =================
        public async Task UpdateAsync(VillaDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("UpdateAsync called with null dto");
                throw new ArgumentNullException(nameof(dto));
            }

            try
            {
                await _inner.UpdateAsync(dto);

                InvalidateCache(dto.Id);

                _logger.LogInformation("Villa {Id} updated and cache invalidated", dto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Villa {Id}", dto.Id);
                throw;
            }
        }

        // ================= CACHE INVALIDATION =================
        private void InvalidateCache(int id)
        {
            try
            {
                _cache.Remove(KEY_ALL);
                _cache.Remove(GetKey(id));

                _logger.LogInformation("Cache invalidated for Villa {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating cache for Villa {Id}", id);
            }
        }
    }
}