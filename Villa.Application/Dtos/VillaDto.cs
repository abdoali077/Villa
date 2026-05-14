using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villla.Domain.Entities;

namespace Villla.Application.Dtos
{
    public class VillaDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public decimal Price { get; set; }
        public int Sqft { get; set; }
        public int Occupancy { get; set; }

        public string? ImageUrl { get; set; }
        public IFormFile? Image { get; set; }

        public List<int>? SelectedAmenities { get; set; }

        public List<AmenityDto>? Amenities { get; set; }
    }
}
