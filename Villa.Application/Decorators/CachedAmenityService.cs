using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Villla.Application.Dtos;
using Villla.Application.Services.Interface;
using Villla.Application.Services.Interface.Cashing;

namespace Villla.Application.Decorators
{
    public class CachedAmenityService : IAmenityService
    {
        private readonly IAmenityService _inner;
        private readonly ICacheService _cache;
        private readonly ILogger<CachedAmenityService> _logger;

        private const string KEY_ALL = "amenities_all";
        private const string KEY_VILLA_LIST = "villa_list";

        private string GetKey(int id) => $"amenity_{id}";

        public CachedAmenityService(
            IAmenityService inner,
            ICacheService cache,
            ILogger<CachedAmenityService> logger)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ================= GET ALL =================
        public async Task<IEnumerable<AmenityDto>> GetAllAsync()
        {
            try
            {
                var cached = _cache.Get<IEnumerable<AmenityDto>>(KEY_ALL);

                if (cached != null)
                {
                    _logger.LogInformation("Cache HIT - Amenities GetAll");
                    return cached;
                }

                _logger.LogInformation("Cache MISS - Amenities GetAll");

                var result = await _inner.GetAllAsync();

                if (result != null)
                {
                    _cache.Set(KEY_ALL, result.ToList(), TimeSpan.FromMinutes(5));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAll Amenities");
                throw;
            }
        }

        // ================= GET BY ID =================
        public async Task<AmenityDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Amenity Id", nameof(id));

            try
            {
                var key = GetKey(id);

                var cached = _cache.Get<AmenityDto>(key);

                if (cached != null)
                {
                    _logger.LogInformation("Cache HIT - Amenity {Id}", id);
                    return cached;
                }

                _logger.LogInformation("Cache MISS - Amenity {Id}", id);

                var result = await _inner.GetByIdAsync(id);

                if (result != null)
                {
                    _cache.Set(key, result, TimeSpan.FromMinutes(5));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetById Amenity {Id}", id);
                throw;
            }
        }

        // ================= VILLA LIST =================
        public async Task<IEnumerable<SelectListItem>> GetVillaListAsync()
        {
            try
            {
                var cached = _cache.Get<IEnumerable<SelectListItem>>(KEY_VILLA_LIST);

                if (cached != null)
                {
                    _logger.LogInformation("Cache HIT - Villa List (Amenity)");
                    return cached;
                }

                var result = await _inner.GetVillaListAsync();

                _cache.Set(KEY_VILLA_LIST, result.ToList(), TimeSpan.FromMinutes(30));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Villa list for Amenity");
                throw;
            }
        }

        // ================= CREATE =================
        public async Task CreateAsync(AmenityDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            try
            {
                await _inner.CreateAsync(dto);

                InvalidateCache(dto.Id);

                _logger.LogInformation("Amenity created and cache invalidated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Amenity {Name}", dto.Name);
                throw;
            }
        }

        // ================= UPDATE =================
        public async Task UpdateAsync(AmenityDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            try
            {
                await _inner.UpdateAsync(dto);

                InvalidateCache(dto.Id);

                _logger.LogInformation("Amenity {Id} updated and cache invalidated", dto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Amenity {Id}", dto.Id);
                throw;
            }
        }

        // ================= DELETE =================
        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Amenity Id", nameof(id));

            try
            {
                await _inner.DeleteAsync(id);

                InvalidateCache(id);

                _logger.LogInformation("Amenity {Id} deleted and cache invalidated", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Amenity {Id}", id);
                throw;
            }
        }

        // ================= CACHE INVALIDATION =================
        private void InvalidateCache(int id)
        {
            try
            {
                _cache.Remove(KEY_ALL);
                _cache.Remove(KEY_VILLA_LIST);
                _cache.Remove(GetKey(id));

                _logger.LogInformation("Amenity cache invalidated for Id {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating Amenity cache for Id {Id}", id);
            }
        }
    }
}