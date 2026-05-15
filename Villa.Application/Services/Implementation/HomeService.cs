using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Villla.Application.Dtos;
using Villla.Application.Interfaces.CommonRepos;
using Villla.Application.Services.Interface;
using Villla.Application.Utility;
using Villla.Domain.Entities;

namespace Villla.Application.Services.Implementation
{
    public class HomeService : IHomeService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<HomeService> _logger;

        public HomeService(IUnitOfWork uow, ILogger<HomeService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        // ================= GET INDEX =================
        public async Task<HomeDto> GetHomeDataAsync(int? villaId, int? scroll)
        {
            try
            {
                _logger.LogInformation("GetHomeData started | Thread: {ThreadId}", Environment.CurrentManagedThreadId);

                var villas = (await _uow.Villas
                    .GetAllAsync(include: q => q.Include(v => v.Amenities)))
                    .ToList();

                _logger.LogInformation("Home villas loaded | Count: {Count}", villas.Count);

                return new HomeDto
                {
                    VillaList = villas.Select(MapToDto),
                    Nights = 1,
                    CheckInDate = DateOnly.FromDateTime(DateTime.Now)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetHomeData");
                throw;
            }
        }

        // ================= POST INDEX =================
        public async Task<HomeDto> FilterHomeDataAsync(HomeDto dto)
        {
            try
            {
                _logger.LogInformation("FilterHomeData started");

                if (dto == null)
                    dto = new HomeDto();

                if (dto.Nights <= 0)
                    dto.Nights = 1;

                if (dto.CheckInDate < DateOnly.FromDateTime(DateTime.Today))
                    dto.CheckInDate = DateOnly.FromDateTime(DateTime.Today);

                var villas = (await _uow.Villas
                    .GetAllAsync(include: q => q.Include(v => v.Amenities)))
                    .ToList();

                dto.VillaList = villas.Select(MapToDto);

                _logger.LogInformation("FilterHomeData completed | Count: {Count}", villas.Count);

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FilterHomeData");
                throw;
            }
        }

        // ================= FILTER BY DATE =================
        public async Task<HomeDto> GetVillasByDateAsync(int nights, DateOnly checkInDate)
        {
            try
            {
                _logger.LogInformation("GetVillasByDate started | Nights: {Nights}, Date: {Date}", nights, checkInDate);

                if (nights <= 0)
                    nights = 1;

                if (checkInDate < DateOnly.FromDateTime(DateTime.Today))
                    checkInDate = DateOnly.FromDateTime(DateTime.Today);

                var villas = (await _uow.Villas
                    .GetAllAsync(include: q => q.Include(v => v.Amenities)))
                    .ToList();

                var villaNumbers = (await _uow.VillaNumbers.GetAllAsync()).ToList();

                var bookings = (await _uow.Bookings.GetAllAsync(
                    b => b.Status == BookingStatus.Approved || b.Status == BookingStatus.CheckIn
                )).ToList();

                foreach (var villa in villas)
                {
                    int availableRooms = BookingAvailabilityHelper
                        .GetAvailableRoomsCount(villa, villaNumbers, checkInDate, nights, bookings);

                    villa.IsAvailable = availableRooms > 0;
                }

                _logger.LogInformation("Availability calculation completed | Villas: {Count}", villas.Count);

                return new HomeDto
                {
                    VillaList = villas.Select(MapToDto),
                    Nights = nights,
                    CheckInDate = checkInDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetVillasByDate");
                throw;
            }
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