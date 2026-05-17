using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Villla.Application.Dtos;
using Villla.Application.Services.Interface;
using Villla.Application.Services.Interface.Cashing;
using Villla.Application.Utility;
using Villla.Domain.Common;

namespace Villla.Application.Decorators
{
    public class CachedVillaNumberService : IVillaNumberService
    {
        private readonly IVillaNumberService _inner;
        private readonly ICacheService _cache;
        private readonly ILogger<CachedVillaNumberService> _logger;

        private const string KEY_ALL = "villanumbers_all";
        private const string KEY_VILLA_LIST = "villa_select_list";

        private string GetKey(int id) => $"villanumber_{id}";

        public CachedVillaNumberService(
            IVillaNumberService inner,
            ICacheService cache,
            ILogger<CachedVillaNumberService> logger)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ================= CREATE =================
        public async Task<bool> CreateAsync(VillaNumberDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            try
            {
                var result = await _inner.CreateAsync(dto);

                if (result)
                {
                    _cache.Remove(KEY_ALL);
                    _cache.Remove(KEY_VILLA_LIST);
                }

                _logger.LogInformation("VillaNumber created successfully and cache invalidated");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating VillaNumber {VillaNumber}", dto.VillaNumber);
                throw;
            }
        }

        // ================= DELETE =================
        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid VillaNumber Id", nameof(id));

            try
            {
                await _inner.DeleteAsync(id);

                InvalidateCache(id);

                _logger.LogInformation("VillaNumber {Id} deleted and cache invalidated", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting VillaNumber {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<VillaNumberDto>> GetAllAsync()
        {
            try
            {
                var cached = _cache.Get<IEnumerable<VillaNumberDto>>(KEY_ALL);

                if (cached != null)
                {
                    _logger.LogInformation("Cache HIT - VillaNumbers GetAll");
                    return cached;
                }

                _logger.LogInformation("Cache MISS - VillaNumbers GetAll");

                var result = await _inner.GetAllAsync();

                if (result != null)
                {
                    _cache.Set(KEY_ALL, result.ToList(), TimeSpan.FromMinutes(5));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAll VillaNumbers");
                throw;
            }
        }

        // ================= GET ALL PAGED =================
        public async Task<PagedResult<VillaNumberDto>> GetAllPagedAsync(PagedRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var cacheKey = CacheKeyHelper.BuildPagedKey("villanumbers", request);
            var cached = _cache.Get<PagedResult<VillaNumberDto>>(cacheKey);

            if (cached != null)
            {
                _logger.LogInformation("Cache HIT - VillaNumbers GetAllPaged | {CacheKey}", cacheKey);
                return cached;
            }

            _logger.LogInformation("Cache MISS - VillaNumbers GetAllPaged | {CacheKey}", cacheKey);

            var result = await _inner.GetAllPagedAsync(request);

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }

        // ================= GET BY ID =================
        public async Task<VillaNumberDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid VillaNumber Id", nameof(id));

            try
            {
                var key = GetKey(id);

                var cached = _cache.Get<VillaNumberDto>(key);

                if (cached != null)
                {
                    _logger.LogInformation("Cache HIT - VillaNumber {Id}", id);
                    return cached;
                }

                _logger.LogInformation("Cache MISS - VillaNumber {Id}", id);

                var result = await _inner.GetByIdAsync(id);

                if (result != null)
                {
                    _cache.Set(key, result, TimeSpan.FromMinutes(5));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetById VillaNumber {Id}", id);
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
                    _logger.LogInformation("Cache HIT - Villa List");
                    return cached;
                }

                var result = await _inner.GetVillaListAsync();

                _cache.Set(KEY_VILLA_LIST, result.ToList(), TimeSpan.FromMinutes(30));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Villa list");
                throw;
            }
        }

        // ================= UPDATE =================
        public async Task UpdateAsync(VillaNumberDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            try
            {
                await _inner.UpdateAsync(dto);

                InvalidateCache(dto.VillaNumber);

                _logger.LogInformation("VillaNumber {Id} updated and cache invalidated", dto.VillaNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating VillaNumber {Id}", dto.VillaNumber);
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
                _cache.RemoveByPrefix("villanumbers_page_");

                _logger.LogInformation("Cache invalidated for VillaNumber {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating cache for VillaNumber {Id}", id);
            }
        }
    }
}