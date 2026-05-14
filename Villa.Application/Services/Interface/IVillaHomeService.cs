using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villla.Application.Dtos;

namespace Villla.Application.Services.Interface
{
    public interface IHomeService
    {
        Task<HomeDto> GetHomeDataAsync(int? villaId, int? scroll);

        Task<HomeDto> FilterHomeDataAsync(HomeDto dto);

        Task<HomeDto> GetVillasByDateAsync(int nights, DateOnly checkInDate);
    }
}
