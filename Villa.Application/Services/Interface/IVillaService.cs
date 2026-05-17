using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villla.Application.Dtos;
using Villla.Domain.Common;

namespace Villla.Application.Services.Interface
{
    public interface IVillaService
    {
        Task<IEnumerable<VillaDto>> GetAllAsync();
        Task<PagedResult<VillaDto>> GetAllPagedAsync(PagedRequest request);
        Task<VillaDto?> GetByIdAsync(int id);

        Task CreateAsync(VillaDto dto);
        Task UpdateAsync(VillaDto dto);

        Task DeleteAsync(int id);
    }

}
