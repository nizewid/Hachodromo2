using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hachodromo.Shared.DTOs
{
    public class TimeSlotDto
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }

        public string Label => $"{Start:hh\\:mm} - {End:hh\\:mm}";
        public int AvailableCount { get; set; }
        public bool IsReservable => AvailableCount > 0;
    }
}
