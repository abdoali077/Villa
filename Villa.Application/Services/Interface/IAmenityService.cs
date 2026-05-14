using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villla.Application.Dtos;

namespace Villla.Application.Services.Interface
{
    public interface IAmenityService
    {
        Task<IEnumerable<AmenityDto>> GetAllAsync();
        Task<AmenityDto?> GetByIdAsync(int id);

        Task<IEnumerable<SelectListItem>> GetVillaListAsync();

        Task CreateAsync(AmenityDto dto);
        Task UpdateAsync(AmenityDto dto);
        Task DeleteAsync(int id);
    }
}
