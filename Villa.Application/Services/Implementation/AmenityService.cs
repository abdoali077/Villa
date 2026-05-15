using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Villla.Application.Dtos;
using Villla.Application.Interfaces.CommonRepos;
using Villla.Application.Services.Interface;
using Villla.Domain.Entities;

namespace Villla.Application.Services.Implementation
{
    public class AmenityService : IAmenityService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AmenityService> _logger;

        public AmenityService(IUnitOfWork uow, ILogger<AmenityService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        // ================= GET ALL =================
        public async Task<IEnumerable<AmenityDto>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("AmenityService - GetAll started");

                var amenities = await _uow.Amenities.GetAllAsync(
                    include: q => q.Include(a => a.Villa)
                );

                var result = amenities.Select(a => new AmenityDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    VillaId = a.VillaId,
                    VillaName = a.Villa != null ? a.Villa.Name : "N/A",
                    Description = a.Description
                });

                _logger.LogInformation("AmenityService - GetAll completed successfully");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting amenities");
                throw;
            }
        }

        // ================= GET BY ID =================
        public async Task<AmenityDto?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("AmenityService - GetById started for Id {Id}", id);

                var amenity = await _uow.Amenities.GetAsync(
                    a => a.Id == id,
                    include: q => q.Include(a => a.Villa)
                );

                if (amenity == null)
                {
                    _logger.LogWarning("AmenityService - Amenity not found for Id {Id}", id);
                    return null;
                }

                var dto = new AmenityDto
                {
                    Id = amenity.Id,
                    Name = amenity.Name,
                    VillaId = amenity.VillaId,
                    VillaName = amenity.Villa != null ? amenity.Villa.Name : "N/A",
                    Description = amenity.Description,
                    VillaList = (await _uow.Villas.GetAllAsync())
                        .Select(v => new SelectListItem
                        {
                            Text = v.Name,
                            Value = v.Id.ToString(),
                            Selected = v.Id == amenity.VillaId
                        })
                };

                _logger.LogInformation("AmenityService - GetById completed for Id {Id}", id);

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting amenity by Id {Id}", id);
                throw;
            }
        }

        // ================= VILLA LIST =================
        public async Task<IEnumerable<SelectListItem>> GetVillaListAsync()
        {
            try
            {
                _logger.LogInformation("AmenityService - GetVillaList started");

                var villas = await _uow.Villas.GetAllAsync();

                var result = villas.Select(v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                });

                _logger.LogInformation("AmenityService - GetVillaList completed");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting villa list for amenities");
                throw;
            }
        }

        // ================= CREATE =================
        public async Task CreateAsync(AmenityDto dto)
        {
            try
            {
                _logger.LogInformation("AmenityService - Create started for {Name}", dto.Name);

                var amenity = new Amenity
                {
                    Name = dto.Name,
                    VillaId = dto.VillaId,
                    Description = dto.Description
                };

                await _uow.Amenities.CreateAsync(amenity);
                await _uow.SaveAsync();

                _logger.LogInformation("AmenityService - Create completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating amenity");
                throw;
            }
        }

        // ================= UPDATE =================
        public async Task UpdateAsync(AmenityDto dto)
        {
            try
            {
                _logger.LogInformation("AmenityService - Update started for Id {Id}", dto.Id);

                var existing = await _uow.Amenities.GetAsync(a => a.Id == dto.Id);

                if (existing == null)
                {
                    _logger.LogWarning("AmenityService - Amenity not found for update Id {Id}", dto.Id);
                    return;
                }

                existing.Name = dto.Name;
                existing.Description = dto.Description;
                existing.VillaId = dto.VillaId;

                await _uow.Amenities.UpdateAmenityAsync(existing);
                await _uow.SaveAsync();

                _logger.LogInformation("AmenityService - Update completed for Id {Id}", dto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating amenity Id {Id}", dto.Id);
                throw;
            }
        }

        // ================= DELETE =================
        public async Task DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("AmenityService - Delete started for Id {Id}", id);

                var amenity = await _uow.Amenities.GetAsync(a => a.Id == id);

                if (amenity == null)
                {
                    _logger.LogWarning("AmenityService - Amenity not found for delete Id {Id}", id);
                    return;
                }

                _uow.Amenities.Delete(amenity);
                await _uow.SaveAsync();

                _logger.LogInformation("AmenityService - Delete completed for Id {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting amenity Id {Id}", id);
                throw;
            }
        }
    }
}