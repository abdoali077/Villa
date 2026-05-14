using Microsoft.EntityFrameworkCore;
using Villla.Application.Dtos;
using Villla.Application.Interfaces.CommonRepos;
using Villla.Application.Interfaces.Services;
using Villla.Application.Services.Interface;
using Villla.Application.Utility;
using Villla.Domain.Entities;

namespace Villla.Application.Services.Implementation
{
    public class HomeService : IHomeService
    {
        private readonly IUnitOfWork _uow;

        public HomeService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // ================= GET INDEX =================
        public async Task<HomeDto> GetHomeDataAsync(int? villaId, int? scroll)
        {
            var villas = _uow.Villas
                .GetAll(include: q => q.Include(v => v.Amenities))
                .ToList();

            return new HomeDto
            {
                VillaList = villas.Select(MapToDto),
                Nights = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now)
            };
        }

        // ================= POST INDEX =================
        public async Task<HomeDto> FilterHomeDataAsync(HomeDto dto)
        {
            if (dto == null)
                dto = new HomeDto();

            if (dto.Nights <= 0)
                dto.Nights = 1;

            if (dto.CheckInDate < DateOnly.FromDateTime(DateTime.Today))
                dto.CheckInDate = DateOnly.FromDateTime(DateTime.Today);

            var villas = _uow.Villas
                .GetAll(include: q => q.Include(v => v.Amenities))
                .ToList();

            dto.VillaList = villas.Select(MapToDto);

            return dto;
        }

        // ================= FILTER BY DATE =================
        public async Task<HomeDto> GetVillasByDateAsync(int nights, DateOnly checkInDate)
        {
            if (nights <= 0)
                nights = 1;

            if (checkInDate < DateOnly.FromDateTime(DateTime.Today))
                checkInDate = DateOnly.FromDateTime(DateTime.Today);

            var villas = _uow.Villas.GetAll(include:
                q => q.Include(v => v.Amenities)).ToList();

            var villaNumbers = _uow.VillaNumbers.GetAll().ToList();

            var bookings = _uow.Bookings
                .GetAll(b => b.Status == BookingStatus.Approved || b.Status == BookingStatus.CheckIn)
                .ToList();

            foreach (var villa in villas)
            {
                int availableRooms = BookingAvailabilityHelper
                    .GetAvailableRoomsCount(villa, villaNumbers, checkInDate, nights, bookings);

                villa.IsAvailable = availableRooms > 0;
            }

            return new HomeDto
            {
                VillaList = villas.Select(MapToDto),
                Nights = nights,
                CheckInDate = checkInDate
            };
        }

        // ================= MAPPING =================
        private VillaDto MapToDto(Villa v)
        {
            return new VillaDto
            {
                Id = v.Id,
                Name = v.Name,
                Description = v.Description,
                Price = v.Price,
                Sqft = v.Sqft,
                Occupancy = v.Occupancy,
                ImageUrl = v.ImageUrl,
                IsAvailable = v.IsAvailable,
                Amenities = v.Amenities?.Select(a => new AmenityDto
                {
                    Id = a.Id,
                    Name = a.Name
                }).ToList()
            };
        }
    }
}