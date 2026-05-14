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
    public class VillaNumberService : IVillaNumberService
    {
        private readonly IUnitOfWork _uow;
        public VillaNumberService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> CreateAsync(VillaNumberDto dto)
        {
            bool exists = _uow.VillaNumbers.GetAll()
                .Any(v => v.Villa_Number == dto.VillaNumber);

            if (exists) return false;

            var entity = new VillaNumber
            {
                Villa_Number = dto.VillaNumber,
                VillaId = dto.VillaId,
                SpecialDetails = dto.SpecialDetails
            };

            _uow.VillaNumbers.Create(entity);
            _uow.Save();

            return true;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = _uow.VillaNumbers.Get(v => v.Villa_Number == id);
            if (entity == null) return;
            _uow.VillaNumbers.Delete(entity);
            _uow.Save();

        }

        public async Task<IEnumerable<VillaNumberDto>> GetAllAsync()
        {
            var data = _uow.VillaNumbers.GetAll(include: q => q.Include(x => x.Villa));
            var model = data.Select(x => new VillaNumberDto
            {
                VillaNumber = x.Villa_Number,
                VillaId = x.VillaId,
                VillaName = x.Villa != null ? x.Villa.Name : "",
                SpecialDetails = x.SpecialDetails,
                VillaList = _uow.Villas.GetAll().Select(v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                }).ToList()
            });
            
            return model;
        }

        public async Task<VillaNumberDto?> GetByIdAsync(int id)
        {
            var data = _uow.VillaNumbers.Get(
                x => x.Villa_Number == id,
                include: q => q.Include(x => x.Villa)
            );
            if (data == null)
                return null;
            var model = new VillaNumberDto
            {
                VillaNumber = data.Villa_Number,
                VillaId = data.VillaId,
                VillaName = data.Villa != null ? data.Villa.Name : "",
                SpecialDetails = data.SpecialDetails,
                VillaList = _uow.Villas.GetAll().Select(v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                }).ToList()
            };
            return model;
        }

        public async Task<IEnumerable<SelectListItem>> GetVillaListAsync()
        {
            var villas =  _uow.Villas.GetAll();
            return villas.Select(v => new SelectListItem
            {
                Text = v.Name,
                Value = v.Id.ToString()
            }).ToList();
            
        }

        public async Task UpdateAsync(VillaNumberDto dto)
        {
            var existing = _uow.VillaNumbers.Get(v => v.Villa_Number == dto.VillaNumber);

            if (existing == null) return;

            existing.VillaId = dto.VillaId;
            existing.SpecialDetails = dto.SpecialDetails;

            _uow.Save();
        }
    }
}
