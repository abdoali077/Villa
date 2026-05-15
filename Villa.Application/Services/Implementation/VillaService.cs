using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Villla.Application.Dtos;
using Villla.Application.Interfaces.CommonRepos;
using Villla.Application.Services.Interface;
using Villla.Domain.Entities;

namespace Villla.Application.Services.Implementation
{
    public class VillaService : IVillaService
    {
        private readonly IUnitOfWork _uow;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<VillaService> _logger;

        public VillaService(IUnitOfWork uow, IWebHostEnvironment env, ILogger<VillaService> logger)
        {
            _uow = uow;
            _env = env;
            _logger = logger;
        }

        // ================= GET ALL =================
        public async Task<IEnumerable<VillaDto>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("GetAll Villas started | Thread: {ThreadId}", Environment.CurrentManagedThreadId);

                var villas = await _uow.Villas.GetAllAsync();

                var result = villas.Select(v => new VillaDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    Description = v.Description,
                    Price = v.Price,
                    Sqft = v.Sqft,
                    Occupancy = v.Occupancy,
                    ImageUrl = v.ImageUrl
                });

                _logger.LogInformation("GetAll Villas completed | Count: {Count}", result.Count());

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAll Villas");
                throw;
            }
        }

        // ================= GET BY ID =================
        public async Task<VillaDto?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("GetVillaById started | VillaId: {VillaId}", id);

                var villa = await _uow.Villas.GetAsync(
                    v => v.Id == id,
                    include: q => q.Include(x => x.Amenities)
                );

                if (villa == null)
                {
                    _logger.LogWarning("Villa not found | VillaId: {VillaId}", id);
                    return null;
                }

                var dto = new VillaDto
                {
                    Id = villa.Id,
                    Name = villa.Name,
                    Description = villa.Description,
                    Price = villa.Price,
                    Sqft = villa.Sqft,
                    Occupancy = villa.Occupancy,
                    ImageUrl = villa.ImageUrl,
                    Amenities = villa.Amenities?
                        .Select(a => new AmenityDto
                        {
                            Id = a.Id,
                            Name = a.Name
                        }).ToList()
                };

                _logger.LogInformation("GetVillaById success | VillaId: {VillaId}", id);

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetVillaById | VillaId: {VillaId}", id);
                throw;
            }
        }

        // ================= CREATE =================
        public async Task CreateAsync(VillaDto dto)
        {
            try
            {
                _logger.LogInformation("Create Villa started | Name: {Name}", dto.Name);

                var villa = new Villa
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    Sqft = dto.Sqft,
                    Occupancy = dto.Occupancy
                };

                villa.ImageUrl = dto.Image != null
                    ? SaveImage(dto.Image)
                    : "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267";

                if (dto.SelectedAmenities != null && dto.SelectedAmenities.Any())
                {
                    var amenities = await Task.WhenAll(
                        dto.SelectedAmenities.Select(id =>
                            _uow.Amenities.GetAsync(a => a.Id == id))
                    );

                    villa.Amenities = amenities
                        .Where(a => a != null)
                        .ToList()!;
                }

                await _uow.Villas.CreateAsync(villa);
                await _uow.SaveAsync();

                _logger.LogInformation("Villa created successfully | Name: {Name}", dto.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating villa | Name: {Name}", dto.Name);
                throw;
            }
        }

        // ================= UPDATE =================
        public async Task UpdateAsync(VillaDto dto)
        {
            try
            {
                _logger.LogInformation("Update Villa started | Id: {Id}", dto.Id);

                var villa = await _uow.Villas.GetAsync(
                    v => v.Id == dto.Id,
                    include: q => q.Include(x => x.Amenities)
                );

                if (villa == null)
                {
                    _logger.LogWarning("Villa not found for update | Id: {Id}", dto.Id);
                    return;
                }

                villa.Name = dto.Name;
                villa.Description = dto.Description;
                villa.Price = dto.Price;
                villa.Sqft = dto.Sqft;
                villa.Occupancy = dto.Occupancy;

                if (dto.Image != null)
                {
                    DeleteImage(villa.ImageUrl);
                    villa.ImageUrl = SaveImage(dto.Image);
                }

                villa.Amenities?.Clear();

                if (dto.SelectedAmenities != null && dto.SelectedAmenities.Any())
                {
                    var amenities = await Task.WhenAll(
                        dto.SelectedAmenities.Select(id =>
                            _uow.Amenities.GetAsync(a => a.Id == id))
                    );

                    villa.Amenities = amenities
                        .Where(a => a != null)
                        .ToList()!;
                }

                await _uow.Villas.UpdateVillaAsync(villa);
                await _uow.SaveAsync();

                _logger.LogInformation("Villa updated successfully | Id: {Id}", dto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating villa | Id: {Id}", dto.Id);
                throw;
            }
        }

        // ================= DELETE =================
        public async Task DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Delete Villa started | Id: {Id}", id);

                var villa = await _uow.Villas.GetAsync(v => v.Id == id);

                if (villa == null)
                {
                    _logger.LogWarning("Villa not found for delete | Id: {Id}", id);
                    return;
                }

                DeleteImage(villa.ImageUrl);

                _uow.Villas.Delete(villa);
                await _uow.SaveAsync();

                _logger.LogInformation("Villa deleted successfully | Id: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting villa | Id: {Id}", id);
                throw;
            }
        }

        // ================= SAVE IMAGE =================
        private string SaveImage(IFormFile file)
        {
            try
            {
                _logger.LogInformation("Saving image | File: {FileName}", file.FileName);

                string wwwRootPath = _env.WebRootPath;
                string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string folderPath = Path.Combine(wwwRootPath, "images", "VillaImage");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var path = "/images/VillaImage/" + fileName;

                _logger.LogInformation("Image saved successfully | Path: {Path}", path);

                return path;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving image");
                throw;
            }
        }

        // ================= DELETE IMAGE =================
        private void DeleteImage(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                    return;

                var relativePath = imageUrl.TrimStart('/')
                    .Replace("/", Path.DirectorySeparatorChar.ToString());

                var path = Path.Combine(_env.WebRootPath, relativePath);

                if (File.Exists(path))
                {
                    File.Delete(path);
                    _logger.LogInformation("Image deleted | Path: {Path}", path);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image | Url: {ImageUrl}", imageUrl);
            }
        }
    }
}