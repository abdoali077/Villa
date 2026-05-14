using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villla.Application.Dtos;
using Villla.Application.Interfaces.CommonRepos;
using Villla.Application.Services.Interface;
using Villla.Domain.Entities;

namespace Villla.Application.Services.Implementation
{
    public class AmenityService : IAmenityService
    {
        private readonly IUnitOfWork _uow;

        public AmenityService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<AmenityDto>> GetAllAsync()
        {
            var amenities = _uow.Amenities.GetAll(
                include: q => q.Include(a => a.Villa)
            );

            return amenities.Select(a => new AmenityDto
            {
                Id = a.Id,
                Name = a.Name,
                VillaId = a.VillaId,
                VillaName = a.Villa != null ? a.Villa.Name : "N/A",
                Description = a.Description
            });
        }
        public async Task<AmenityDto?> GetByIdAsync(int id)
        {
            var amenity = _uow.Amenities.Get(
                a => a.Id == id,
                include: q => q.Include(a => a.Villa)
            );
            if (amenity == null)
                return null;
            return new AmenityDto
            {
                Id = amenity.Id,
                Name = amenity.Name,
                VillaId = amenity.VillaId,
                VillaName = amenity.Villa != null ? amenity.Villa.Name : "N/A",
                Description = amenity.Description,
                    VillaList = _uow.Villas.GetAll().Select(v => new SelectListItem
                    {
                        Text = v.Name,
                        Value = v.Id.ToString(),
                        Selected = v.Id == amenity.VillaId
                    })

            };
        }
        public async Task<IEnumerable<SelectListItem>> GetVillaListAsync()
        {
            return _uow.Villas.GetAll().Select(v => new SelectListItem
            {
                Text = v.Name,
                Value = v.Id.ToString()
            });
        }

        public async Task CreateAsync(AmenityDto dto)
        {
            var amenity = new Amenity
            {
                Name = dto.Name,
                VillaId = dto.VillaId,
                Description = dto.Description,

            };

            _uow.Amenities.Create(amenity);
            _uow.Save();
        }

        public async Task UpdateAsync(AmenityDto dto)
        {
            var existing = _uow.Amenities.Get(a => a.Id == dto.Id);

            if (existing == null) return;

            existing.Name = dto.Name;

            _uow.Amenities.UpdateAmenity(existing);
            _uow.Save();
        }

        public async Task DeleteAsync(int id)
        {
            var amenity = _uow.Amenities.Get(a => a.Id == id);

            if (amenity == null) return;

            _uow.Amenities.Delete(amenity);
            _uow.Save();
        }
    }
}
