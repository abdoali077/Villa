using Microsoft.AspNetCore.Mvc.Rendering;
using Villla.Application.Dtos;

namespace Villla.Application.Services.Interface
{
    public interface IVillaNumberService
    {
        Task<IEnumerable<VillaNumberDto>> GetAllAsync();
        Task<VillaNumberDto?> GetByIdAsync(int id);

        Task<IEnumerable<SelectListItem>> GetVillaListAsync();

        Task<bool> CreateAsync(VillaNumberDto dto);
        Task UpdateAsync(VillaNumberDto dto);
        Task DeleteAsync(int id);
    }
}
