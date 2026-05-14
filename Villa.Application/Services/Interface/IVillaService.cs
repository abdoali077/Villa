using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villla.Application.Dtos;

namespace Villla.Application.Services.Interface
{
    public interface IVillaService
    {
        Task<IEnumerable<VillaDto>> GetAllAsync();
        Task<VillaDto?> GetByIdAsync(int id);

        Task CreateAsync(VillaDto dto);
        Task UpdateAsync(VillaDto dto);

        Task DeleteAsync(int id);
    }

}
