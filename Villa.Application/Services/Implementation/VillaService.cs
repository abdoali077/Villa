using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

        public VillaService(IUnitOfWork uow, IWebHostEnvironment env)
        {
            _uow = uow;
            _env = env;
        }

        // ================= GET ALL =================
        public Task<IEnumerable<VillaDto>> GetAllAsync()
        {
            var villas = _uow.Villas.GetAll();

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

            return Task.FromResult(result);
        }

        // ================= GET BY ID =================
        public Task<VillaDto?> GetByIdAsync(int id)
        {
            var villa = _uow.Villas.Get(
                v => v.Id == id,
                include: q => q.Include(x => x.Amenities)
            );

            if (villa == null)
                return Task.FromResult<VillaDto?>(null);

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

            return Task.FromResult<VillaDto?>(dto);
        }

        // ================= CREATE =================
        public Task CreateAsync(VillaDto dto)
        {
            var villa = new Villa
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Sqft = dto.Sqft,
                Occupancy = dto.Occupancy
            };

            // Image upload
            villa.ImageUrl = dto.Image != null
                ? SaveImage(dto.Image)
                : "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267";

            // Amenities (IMPORTANT FIX)
            if (dto.SelectedAmenities != null && dto.SelectedAmenities.Any())
            {
                villa.Amenities = dto.SelectedAmenities
                     .Select(id =>
                     {
                         var amenity = _uow.Amenities.Get(a => a.Id == id);

                         return new Amenity
                         {
                             Id = amenity.Id,
                             Name = amenity.Name
                         };
                     })
                     .ToList();
            }

            _uow.Villas.Create(villa);
            _uow.Save();

            return Task.CompletedTask;
        }

        // ================= UPDATE =================
        public Task UpdateAsync(VillaDto dto)
        {
            var villa = _uow.Villas.Get(
                v => v.Id == dto.Id,
                include: q => q.Include(x => x.Amenities)
            );

            if (villa == null)
                return Task.CompletedTask;

            villa.Name = dto.Name;
            villa.Description = dto.Description;
            villa.Price = dto.Price;
            villa.Sqft = dto.Sqft;
            villa.Occupancy = dto.Occupancy;

            // Image update
            if (dto.Image != null)
            {
                DeleteImage(villa.ImageUrl);
                villa.ImageUrl = SaveImage(dto.Image);
            }

            // Amenities update (IMPORTANT FIX)
            villa.Amenities?.Clear();

            if (dto.SelectedAmenities != null)
            {
                villa.Amenities = dto.SelectedAmenities
                     .Select(id =>
                     {
                         var amenity = _uow.Amenities.Get(a => a.Id == id);

                         return new Amenity
                         {
                             Id = amenity.Id,
                             Name = amenity.Name
                         };
                     })
                     .ToList();
            }

            _uow.Villas.UpdateVilla(villa);
            _uow.Save();

            return Task.CompletedTask;
        }

        // ================= DELETE =================
        public Task DeleteAsync(int id)
        {
            var villa = _uow.Villas.Get(v => v.Id == id);

            if (villa == null)
                return Task.CompletedTask;

            DeleteImage(villa.ImageUrl);

            _uow.Villas.Delete(villa);
            _uow.Save();

            return Task.CompletedTask;
        }

        // ================= HELPERS =================
        private string SaveImage(IFormFile file)
        {
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

            return "/images/VillaImage/" + fileName;
        }

        private void DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return;

            var relativePath = imageUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());

            var path = Path.Combine(_env.WebRootPath, relativePath);

            if (File.Exists(path))
                File.Delete(path);
        }
    }
}