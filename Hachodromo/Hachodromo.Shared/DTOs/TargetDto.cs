using Hachodromo.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hachodromo.Shared.DTOs
{
    public class TargetDto
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public int Capacity { get; set; }
        public TargetStatus Status { get; set; }
    }

}
