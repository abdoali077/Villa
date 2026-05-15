using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Villla.Application.Dtos;
using Villla.Application.Interfaces.CommonRepos;
using Villla.Application.Services.Interface;
using Villla.Domain.Entities;

namespace Villla.Application.Services.Implementation
{
    public class VillaNumberService : IVillaNumberService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<VillaNumberService> _logger;

        public VillaNumberService(IUnitOfWork uow, ILogger<VillaNumberService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        // ================= CREATE =================
        public async Task<bool> CreateAsync(VillaNumberDto dto)
        {
            try
            {
                _logger.LogInformation("Create VillaNumber started | VillaNumber: {VillaNumber}", dto.VillaNumber);

                var exists = (await _uow.VillaNumbers.GetAllAsync())
                    .Any(v => v.Villa_Number == dto.VillaNumber);

                if (exists)
                {
                    _logger.LogWarning("VillaNumber already exists | VillaNumber: {VillaNumber}", dto.VillaNumber);
                    return false;
                }

                var entity = new VillaNumber
                {
                    Villa_Number = dto.VillaNumber,
                    VillaId = dto.VillaId,
                    SpecialDetails = dto.SpecialDetails
                };

                await _uow.VillaNumbers.CreateAsync(entity);
                await _uow.SaveAsync();

                _logger.LogInformation("VillaNumber created successfully | VillaNumber: {VillaNumber}", dto.VillaNumber);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating VillaNumber | VillaNumber: {VillaNumber}", dto.VillaNumber);
                throw;
            }
        }

        // ================= DELETE =================
        public async Task DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Delete VillaNumber started | VillaNumber: {VillaNumber}", id);

                var entity = await _uow.VillaNumbers.GetAsync(v => v.Villa_Number == id);

                if (entity == null)
                {
                    _logger.LogWarning("VillaNumber not found for delete | VillaNumber: {VillaNumber}", id);
                    return;
                }

                _uow.VillaNumbers.Delete(entity);
                await _uow.SaveAsync();

                _logger.LogInformation("VillaNumber deleted successfully | VillaNumber: {VillaNumber}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting VillaNumber | VillaNumber: {VillaNumber}", id);
                throw;
            }
        }

        // ================= GET ALL =================
        public async Task<IEnumerable<VillaNumberDto>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("GetAll VillaNumbers started");

                var villaNumbers = await _uow.VillaNumbers
                    .GetAllAsync(include: q => q.Include(x => x.Villa));

                var villas = await _uow.Villas.GetAllAsync();

                var villaList = villas.Select(v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                }).ToList();

                var result = villaNumbers.Select(x => new VillaNumberDto
                {
                    VillaNumber = x.Villa_Number,
                    VillaId = x.VillaId,
                    VillaName = x.Villa?.Name ?? "",
                    SpecialDetails = x.SpecialDetails,
                    VillaList = villaList
                });

                _logger.LogInformation("GetAll VillaNumbers completed | Count: {Count}", result.Count());

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all VillaNumbers");
                throw;
            }
        }

        // ================= GET BY ID =================
        public async Task<VillaNumberDto?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Get VillaNumber by ID started | VillaNumber: {VillaNumber}", id);

                var data = await _uow.VillaNumbers.GetAsync(
                    x => x.Villa_Number == id,
                    include: q => q.Include(x => x.Villa)
                );

                if (data == null)
                {
                    _logger.LogWarning("VillaNumber not found | VillaNumber: {VillaNumber}", id);
                    return null;
                }

                var model = new VillaNumberDto
                {
                    VillaNumber = data.Villa_Number,
                    VillaId = data.VillaId,
                    VillaName = data.Villa?.Name ?? "",
                    SpecialDetails = data.SpecialDetails,
                    VillaList = (await _uow.Villas.GetAllAsync())
                        .Select(v => new SelectListItem
                        {
                            Text = v.Name,
                            Value = v.Id.ToString()
                        }).ToList()
                };

                _logger.LogInformation("Get VillaNumber by ID success | VillaNumber: {VillaNumber}", id);

                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting VillaNumber by ID | VillaNumber: {VillaNumber}", id);
                throw;
            }
        }

        // ================= VILLA LIST =================
        public async Task<IEnumerable<SelectListItem>> GetVillaListAsync()
        {
            try
            {
                _logger.LogInformation("GetVillaList started");

                var villas = await _uow.Villas.GetAllAsync();

                var result = villas.Select(v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                }).ToList();

                _logger.LogInformation("GetVillaList completed | Count: {Count}", result.Count);

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
            try
            {
                _logger.LogInformation("Update VillaNumber started | VillaNumber: {VillaNumber}", dto.VillaNumber);

                var existing = await _uow.VillaNumbers.GetAsync(v => v.Villa_Number == dto.VillaNumber);

                if (existing == null)
                {
                    _logger.LogWarning("VillaNumber not found for update | VillaNumber: {VillaNumber}", dto.VillaNumber);
                    return;
                }

                existing.VillaId = dto.VillaId;
                existing.SpecialDetails = dto.SpecialDetails;

                await _uow.SaveAsync();

                _logger.LogInformation("VillaNumber updated successfully | VillaNumber: {VillaNumber}", dto.VillaNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating VillaNumber | VillaNumber: {VillaNumber}", dto.VillaNumber);
                throw;
            }
        }
    }
}