using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villla.Application.Dtos
{
    public class HomeDto
    {
        public IEnumerable<VillaDto> VillaList { get; set; } = new List<VillaDto>();

        public DateOnly CheckOutDate { get; set; }

        public DateOnly? CheckInDate { get; set; }

        public int Nights { get; set; }
    }
}
